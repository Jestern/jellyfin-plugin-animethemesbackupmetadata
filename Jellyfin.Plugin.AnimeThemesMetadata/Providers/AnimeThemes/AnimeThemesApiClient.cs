using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Jellyfin.Plugin.AnimeThemesMetadata.Providers.AnimeThemes;

#pragma warning disable CS8603 // Possible null reference return.

public class AnimeThemesApiClient
{
    private const string AnimeThemesApiUrl = "https://graphql.animethemes.moe/";

    private const string GetMusicVideoGraphqlQuery = """
        query ($filename : String) {
            videoPaginator(filename : $filename) {
                data {
                    id
                    filename
                    lyrics
                    tags
                    source
                    animethemeentries(first: 1) {
                        nodes {
                            animetheme {
                                type
                                slug
                                anime {
                                slug
                                name
                                year
                                season
                                synopsis
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
                                                    id
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
    """;

    private const string GetArtistGraphqlQuery = """
        query ($name : String) {
            artistPaginator(name : $name) {
                data {
                    id
                    name
                    slug
                    images {
                        edges {
                            node {
                                facet
                                link
                            }
                        }
                    }
                }
            }
        }
    """;

    public async Task<VideoDto> GetMusicVideo(string filename, CancellationToken cancellationToken)
    {
        var result = await WebRequestAPI(
            new GraphQlRequest()
            {
                Query = GetMusicVideoGraphqlQuery,
                Variables = new Dictionary<string, string> { { "filename", filename } }
            },
            cancellationToken).ConfigureAwait(false);

        return result.Data?.VideoPaginator?.Data.FirstOrDefault();
    }

    public async Task<ArtistDto> GetArtist(string name, CancellationToken cancellationToken)
    {
        var result = await WebRequestAPI(
            new GraphQlRequest()
            {
                Query = GetArtistGraphqlQuery,
                Variables = new Dictionary<string, string> { { "name", name } }
            },
            cancellationToken).ConfigureAwait(false);

        return result.Data?.ArtistPaginator?.Data.FirstOrDefault();
    }

    private async Task<RootDto> WebRequestAPI(GraphQlRequest request, CancellationToken cancellationToken)
    {
        var httpClient = Plugin.Instance.GetHttpClient();

        using HttpContent content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        using var response = await httpClient.PostAsync(AnimeThemesApiUrl, content, cancellationToken).ConfigureAwait(false);
        using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);

        return await JsonSerializer.DeserializeAsync<RootDto>(responseStream, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    private sealed class GraphQlRequest
    {
        [JsonPropertyName("query")]
        public string Query { get; set; }

        [JsonPropertyName("variables")]
        public Dictionary<string, string> Variables { get; set; }
    }
}
