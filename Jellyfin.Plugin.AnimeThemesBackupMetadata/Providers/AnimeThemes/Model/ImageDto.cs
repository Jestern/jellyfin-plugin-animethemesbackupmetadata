using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.AnimeThemesBackupMetadata.Providers.AnimeThemes.Model;

public class ImageDto
{
    [JsonPropertyName("facet")]
    public string Facet { get; set; }

    [JsonPropertyName("link")]
    public string Link { get; set; }
}
