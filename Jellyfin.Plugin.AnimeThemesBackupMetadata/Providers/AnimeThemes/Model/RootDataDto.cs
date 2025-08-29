using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.AnimeThemesBackupMetadata.Providers.AnimeThemes.Model;

public class RootDataDto
{
    [JsonPropertyName("videoPaginator")]
    public VideoPaginatorDto? VideoPaginator { get; set; }

    [JsonPropertyName("search")]
    public SearchDto? Search { get; set; }
}
