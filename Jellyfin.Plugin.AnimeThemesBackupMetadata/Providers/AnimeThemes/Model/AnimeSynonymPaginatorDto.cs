using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.AnimeThemesBackupMetadata.Providers.AnimeThemes.Model;

public class AnimeSynonymPaginatorDto
{
    [JsonPropertyName("data")]
    public List<AnimeSynonymDto> Data { get; set; }
}
