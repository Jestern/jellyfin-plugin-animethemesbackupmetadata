using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.AnimeThemesBackupMetadata.Providers.AnimeThemes.Model;

public class RootDto
{
    [JsonPropertyName("data")]
    public RootDataDto? Data { get; set; }
}
