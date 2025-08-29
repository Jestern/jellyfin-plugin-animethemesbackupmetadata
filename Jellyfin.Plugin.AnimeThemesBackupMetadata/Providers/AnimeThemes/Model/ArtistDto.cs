using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.AnimeThemesBackupMetadata.Providers.AnimeThemes.Model;

public class ArtistDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("group")]
    public ArtistDto? Group { get; set; }

    [JsonPropertyName("members")]
    public ArtistArtistEdgeConnectionDto? Members { get; set; }
}
