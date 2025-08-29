using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.AnimeThemesBackupMetadata.Providers.AnimeThemes.Model;

public class AnimethemeEntriesPaginatorDto
{
    [JsonPropertyName("data")]
    public List<AnimethemeEntryDto> Data { get; set; }
}
