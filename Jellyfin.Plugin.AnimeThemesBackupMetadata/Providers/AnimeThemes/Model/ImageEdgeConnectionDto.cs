using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Jellyfin.Extensions;

namespace Jellyfin.Plugin.AnimeThemesBackupMetadata.Providers.AnimeThemes.Model;

public class ImageEdgeConnectionDto
{
    [JsonPropertyName("nodes")]
    public List<ImageDto> Nodes { get; set; }

    public string? GetSmallImageLink()
    {
        return Nodes.FirstOrDefault(x => string.Equals(x.Facet, "SMALL_COVER", System.StringComparison.Ordinal))?.Link;
    }

    public string? GetLargeImageLink()
    {
        return Nodes.FirstOrDefault(x => string.Equals(x.Facet, "LARGE_COVER", System.StringComparison.Ordinal))?.Link;
    }
}
