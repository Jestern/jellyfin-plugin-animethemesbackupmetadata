using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using MediaBrowser.Model.Providers;

namespace Jellyfin.Plugin.AnimeThemesBackupMetadata.Providers.AnimeThemes.Model;

public class AnimeDto
{
    [JsonPropertyName("slug")]
    public string Slug { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("year")]
    public int Year { get; set; }

    [JsonPropertyName("season")]
    public string Season { get; set; }

    [JsonPropertyName("images")]
    public ImageEdgeConnectionDto? Images { get; set; }

    [JsonPropertyName("synopsis")]
    public string Synopsis { get; set; }

    [JsonPropertyName("animesynonyms")]
    public AnimeSynonymPaginatorDto? Animesynonyms { get; set; }

    [JsonPropertyName("studios")]
    public AnimeStudioEdgeConnectionDto? Studios { get; set; }

    [JsonPropertyName("animethemes")]
    public AnimethemePaginatorDto? Animethemes { get; set; }

    public List<RemoteSearchResult> ToRemoteSearchResults()
    {
        var result = new List<RemoteSearchResult>();

        if (Animethemes is not null)
        {
            Animethemes.Data.ForEach(at => result.Add(new RemoteSearchResult
            {
                Name = GetName(at),
                ProductionYear = Year,
                ImageUrl = Images?.GetLargeImageLink(),
                SearchProviderName = ProviderNames.AnimeThemes,
                ProviderIds = new Dictionary<string, string>() { { ProviderNames.AnimeThemes, GetVideoId(at) } }
            }));
        }

        return result;
    }

    private string GetVideoId(AnimethemeDto animetheme)
    {
        var video = GetVideo(animetheme);
        return video?.Id.ToString(CultureInfo.InvariantCulture)
            ?? string.Empty;
    }

    private string GetName(AnimethemeDto animetheme)
    {
        var animethemeentries = animetheme.AnimethemeEntries?.Data.FirstOrDefault();
        var video = GetVideo(animetheme);

        if (animethemeentries is null)
        {
            return Name;
        }

        var name = new StringBuilder();

        name.Append(animetheme?.Slug);

        var version = animethemeentries.Version;
        if (version is not null)
        {
            name.AppendFormat(CultureInfo.InvariantCulture, "v{0}", version);
        }

        name.Append(" - ");

        if (!string.IsNullOrEmpty(video?.Tags))
        {
            name.Append(video?.Tags);
            name.Append(" - ");
        }

        name.Append(Name);

        return name.ToString();
    }

    private VideoDto? GetVideo(AnimethemeDto animetheme)
    {
        return animetheme.AnimethemeEntries?.Data.FirstOrDefault()?.Videos?.Nodes.FirstOrDefault();
    }
}
