using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.AnimeThemesBackupMetadata.Providers.AnimeThemes.Model;

public class PerformanceDto
{
    [JsonPropertyName("artist")]
    public ArtistDto? Artist { get; set; }
}
