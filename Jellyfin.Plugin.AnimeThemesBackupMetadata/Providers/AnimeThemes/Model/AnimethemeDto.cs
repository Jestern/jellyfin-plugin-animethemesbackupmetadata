using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.AnimeThemesBackupMetadata.Providers.AnimeThemes.Model;

public class AnimethemeDto
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("slug")]
    public string Slug { get; set; }

    [JsonPropertyName("anime")]
    public AnimeDto? Anime { get; set; }

    [JsonPropertyName("song")]
    public SongDto? Song { get; set; }

    [JsonPropertyName("animethemeentries")]
    public AnimethemeEntriesPaginatorDto? AnimethemeEntries { get; set; }
}
