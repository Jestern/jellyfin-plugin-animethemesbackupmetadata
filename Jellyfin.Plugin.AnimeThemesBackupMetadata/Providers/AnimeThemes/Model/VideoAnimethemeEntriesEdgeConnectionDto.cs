using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.AnimeThemesBackupMetadata.Providers.AnimeThemes.Model;

public class VideoAnimethemeEntriesEdgeConnectionDto
{
    [JsonPropertyName("nodes")]
    public List<AnimethemeEntryDto> Nodes { get; set; }
}
