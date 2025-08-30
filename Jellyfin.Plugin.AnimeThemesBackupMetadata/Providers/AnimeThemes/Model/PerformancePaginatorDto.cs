using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.AnimeThemesBackupMetadata.Providers.AnimeThemes.Model;

public class PerformancePaginatorDto
{
    [JsonPropertyName("data")]
    public List<PerformanceDto> Data { get; set; }

    public string[] GetArtistsNames()
    {
        var artistNames = new HashSet<string>();

        Data.ForEach(a =>
        {
            var group = a.Artist?.Group;
            if (group is not null)
            {
                artistNames.Add(group.Name);
                group.Members?.Nodes.ForEach(m => artistNames.Add(m.Name));
            }
            else if (!string.IsNullOrEmpty(a.Artist?.Name))
            {
                artistNames.Add(a.Artist.Name);
            }
        });

        var result = new string[artistNames.Count];
        artistNames.CopyTo(result);

        return result;
    }
}
