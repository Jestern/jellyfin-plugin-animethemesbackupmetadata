using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.AnimeThemesBackupMetadata.Providers.AnimeThemes.Model;
using Microsoft.Extensions.Logging;
using Polly;

namespace Jellyfin.Plugin.AnimeThemesBackupMetadata.Providers.AnimeThemes;

#pragma warning disable CS8603 // Possible null reference return.

public class AnimeThemesApiClient(ILogger logger)
{
    private const string SearchGraphqlQuery = """
    query ($name : String!) {
        search(search: $name) {
            anime {
                name
                year
                images {
                    nodes {
                        facet
                        link
                    }
                }
                animethemes {
                    data {
                        slug
                        animethemeentries {
                            data {
                            version
                                videos {
                                    nodes {
                                        id
                                        tags
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    """;

    private const string CommonDataMusicVideo = """
    data {
        id
        filename
        lyrics
        tags
        source
        animethemeentries(first: 1) {
            nodes {
                version
                animetheme {
                    type
                    slug
                    anime {
                        slug
                        name
                        year
                        season
                        synopsis
                        images {
                            nodes {
                                facet
                                link
                            }
                        }
                        animesynonyms(first: 10) {
                            data {
                            text
                            }
                        }
                        studios (first: 5) {
                            nodes {
                            name
                            }
                        }
                    }
                    song {
                        title
                        performances(first:10) {
                            data {
                                artist {
                                    ... on Artist {
                                        name
                                    }
                                    ... on Membership {
                                        group {
                                            name
                                            members {
                                              nodes {
                                                name
                                              }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    """;

    private const string GetMusicVideoByFileNameGraphqlQuery = """
        query ($filename : String) {
            videoPaginator(filename : $filename) {
    """
    + CommonDataMusicVideo +
    """
            }
        }
    """;

    private const string GetMusicVideoByIdGraphqlQuery = """
        query ($id : Int) {
            videoPaginator(id : $id) {
    """
    + CommonDataMusicVideo +
    """
            }
        }
    """;

    private readonly ILogger _log = logger;

    public async Task<List<AnimeDto>> SearchByName(string name, CancellationToken cancellationToken)
    {
        var result = await WebRequestAPI(
            new GraphQlRequest()
            {
                Query = SearchGraphqlQuery,
                Variables = new Dictionary<string, dynamic> { { "name", name } }
            },
            cancellationToken).ConfigureAwait(false);

        return result.Data?.Search?.Animes;
    }

    public async Task<VideoDto> GetMusicVideoByFilename(string filename, CancellationToken cancellationToken)
    {
        var result = await WebRequestAPI(
            new GraphQlRequest()
            {
                Query = GetMusicVideoByFileNameGraphqlQuery,
                Variables = new Dictionary<string, dynamic> { { "filename", filename } }
            },
            cancellationToken).ConfigureAwait(false);

        return result.Data?.VideoPaginator?.Data.FirstOrDefault();
    }

    public async Task<VideoDto> GetMusicVideoById(int atId, CancellationToken cancellationToken)
    {
        var result = await WebRequestAPI(
            new GraphQlRequest()
            {
                Query = GetMusicVideoByIdGraphqlQuery,
                Variables = new Dictionary<string, dynamic> { { "id", atId } }
            },
            cancellationToken).ConfigureAwait(false);

        return result.Data?.VideoPaginator?.Data.FirstOrDefault();
    }

    private async Task<RootDto> WebRequestAPI(GraphQlRequest request, CancellationToken cancellationToken)
    {
        var config = Plugin.Instance.Configuration;
        var httpClient = Plugin.Instance.GetHttpClient();

        var retryPolicy = Policy
            .Handle<HttpRequestException>()
            .Or<TaskCanceledException>(_ => !cancellationToken.IsCancellationRequested)
            .Or<OperationCanceledException>(_ => !cancellationToken.IsCancellationRequested)
            .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .WaitAndRetryAsync(
                retryCount: config.MaxRetryAttemps,
                sleepDurationProvider: (_, result, _) =>
                {
                    return result.Result.Headers.RetryAfter?.Delta ?? TimeSpan.FromSeconds(config.Delay);
                },
                onRetryAsync: (_, timespan, retryCount, _) =>
                {
                    _log.LogWarning("AnimeThemes request failed, retrying in {Time}, attempt {Attempt}", timespan, retryCount);

                    return Task.CompletedTask;
                });

        var policyResult = await retryPolicy.ExecuteAndCaptureAsync(
            async ct =>
        {
            using var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            _log.LogDebug("Animethemes request content: {Content}", content);

            var response = await httpClient.PostAsync(config.AnimeThemesGraphqlUrl, content, ct).ConfigureAwait(false);

            return response;
        },
            cancellationToken).ConfigureAwait(false);

        if (policyResult.Outcome is OutcomeType.Failure)
        {
            _log.LogError("Animethemes request failed {Request}", request);

            throw policyResult.FinalException ?? new HttpRequestException("An unknown error has ocurring while making the request.");
        }

        using var responseStream = await policyResult.Result.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        var result = await JsonSerializer.DeserializeAsync<RootDto>(responseStream, cancellationToken: cancellationToken).ConfigureAwait(false);

        return result;
    }

    private sealed class GraphQlRequest
    {
        [JsonPropertyName("query")]
        public string Query { get; set; }

        [JsonPropertyName("variables")]
        public Dictionary<string, dynamic> Variables { get; set; }
    }
}
