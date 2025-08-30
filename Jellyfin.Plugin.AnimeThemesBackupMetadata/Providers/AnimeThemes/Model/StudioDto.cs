using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.AnimeThemesBackupMetadata.Providers.AnimeThemes.Model;

public class StudioDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
}
