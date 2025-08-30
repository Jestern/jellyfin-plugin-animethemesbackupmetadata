using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.AnimeThemesBackupMetadata.Providers.AnimeThemes.Model;

public class VideoPaginatorDto
{
    [JsonPropertyName("data")]
    public List<VideoDto> Data { get; set; }
}
