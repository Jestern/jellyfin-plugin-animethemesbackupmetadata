using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.AnimeThemesBackupMetadata.Providers.AnimeThemes.Model;

public class AnimeStudioEdgeConnectionDto
{
    [JsonPropertyName("nodes")]
    public List<StudioDto> Nodes { get; set; }

    public string[] GetStudiosNames()
    {
        return Nodes.Select(s => s.Name).ToArray();
    }
}
