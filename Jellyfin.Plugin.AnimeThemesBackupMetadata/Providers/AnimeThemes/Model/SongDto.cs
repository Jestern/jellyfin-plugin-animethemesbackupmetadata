using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.AnimeThemesBackupMetadata.Providers.AnimeThemes.Model;

public class SongDto
{
    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("performances")]
    public PerformancePaginatorDto? Performances { get; set; }
}
