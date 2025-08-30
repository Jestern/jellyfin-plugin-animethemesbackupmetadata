using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.AnimeThemesBackupMetadata.Providers.AnimeThemes.Model;

public class AnimeSynonymDto
{
    [JsonPropertyName("text")]
    public string Text { get; set; }
}
