using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.AnimeThemesBackupMetadata.Providers.AnimeThemes.Model;

public class AnimethemeEntryDto
{
    [JsonPropertyName("version")]
    public int? Version { get; set; }

    [JsonPropertyName("animetheme")]
    public AnimethemeDto? Animetheme { get; set; }

    [JsonPropertyName("videos")]
    public AnimeThemeEntryVideoEdgeConnectionDto? Videos { get; set; }
}
