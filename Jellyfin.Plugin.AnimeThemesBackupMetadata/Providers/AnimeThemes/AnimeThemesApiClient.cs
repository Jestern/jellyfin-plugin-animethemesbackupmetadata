using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.AnimeThemesBackupMetadata.Providers.AnimeThemes.Model;

namespace Jellyfin.Plugin.AnimeThemesBackupMetadata.Providers.AnimeThemes;

#pragma warning disable CS8603 // Possible null reference return.

public class AnimeThemesApiClient
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

    public async Task<VideoDto> GetMusicVideoById(string atId, CancellationToken cancellationToken)
    {
        var result = await WebRequestAPI(
            new GraphQlRequest()
            {
                Query = GetMusicVideoByIdGraphqlQuery,
                Variables = new Dictionary<string, dynamic> { { "id", int.Parse(atId, CultureInfo.InvariantCulture) } }
            },
            cancellationToken).ConfigureAwait(false);

        return result.Data?.VideoPaginator?.Data.FirstOrDefault();
    }

    private async Task<RootDto> WebRequestAPI(GraphQlRequest request, CancellationToken cancellationToken)
    {
        var config = Plugin.Instance.Configuration;
        var httpClient = Plugin.Instance.GetHttpClient();

        using HttpContent content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        for (var attempt = 0; attempt < config.MaxRetryAttemps; ++attempt)
        {
            TimeSpan delay = default;
            using (var response = await httpClient.PostAsync(config.AnimeThemesGraphqlUrl, content, cancellationToken).ConfigureAwait(false))
            {
                if (response.IsSuccessStatusCode)
                {
                    using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
                    return await JsonSerializer.DeserializeAsync<RootDto>(responseStream, cancellationToken: cancellationToken).ConfigureAwait(false);
                }

                delay = response.Headers.RetryAfter?.Delta ?? TimeSpan.FromSeconds(config.Delay);
            }

            await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
        }

        throw new HttpRequestException($"Failed to get metadata({request.Variables})");
    }

    private sealed class GraphQlRequest
    {
        [JsonPropertyName("query")]
        public string Query { get; set; }

        [JsonPropertyName("variables")]
        public Dictionary<string, dynamic> Variables { get; set; }
    }
}
