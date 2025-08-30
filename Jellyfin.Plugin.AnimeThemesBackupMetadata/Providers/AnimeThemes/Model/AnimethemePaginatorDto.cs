using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.AnimeThemesBackupMetadata.Providers.AnimeThemes.Model;

public class AnimethemePaginatorDto
{
    [JsonPropertyName("data")]
    public List<AnimethemeDto> Data { get; set; }
}
