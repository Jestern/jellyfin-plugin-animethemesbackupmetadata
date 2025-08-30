using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.AnimeThemesBackupMetadata.Providers.AnimeThemes.Model;

public class ArtistArtistEdgeConnectionDto
{
    [JsonPropertyName("nodes")]
    public List<ArtistDto> Nodes { get; set; }
}
