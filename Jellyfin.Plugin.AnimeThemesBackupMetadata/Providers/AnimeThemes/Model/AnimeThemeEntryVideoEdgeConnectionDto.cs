using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.AnimeThemesBackupMetadata.Providers.AnimeThemes.Model;

public class AnimeThemeEntryVideoEdgeConnectionDto
{
    [JsonPropertyName("nodes")]
    public List<VideoDto> Nodes { get; set; }
}
