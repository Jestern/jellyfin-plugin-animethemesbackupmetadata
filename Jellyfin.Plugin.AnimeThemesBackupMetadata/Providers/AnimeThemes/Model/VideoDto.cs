using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using Jellyfin.Extensions;
using Jellyfin.Plugin.AnimeThemesBackupMetadata.Extensions;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Model.Providers;

namespace Jellyfin.Plugin.AnimeThemesBackupMetadata.Providers.AnimeThemes.Model;

#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

public class VideoDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("filename")]
    public string Filename { get; set; }

    [JsonPropertyName("lyrics")]
    public bool Lyrics { get; set; }

    [JsonPropertyName("tags")]
    public string Tags { get; set; }

    [JsonPropertyName("source")]
    public string Source { get; set; }

    [JsonPropertyName("animethemeentries")]
    public VideoAnimethemeEntriesEdgeConnectionDto? AnimethemeEntries { get; set; }

    public MusicVideo ToMusicVideo()
    {
        var animetheme = AnimethemeEntries?.Nodes.FirstOrDefault()?.Animetheme;
        var anime = animetheme?.Anime;
        var song = animetheme?.Song;

        var result = new MusicVideo()
        {
            Name = GetName(),
            HasSubtitles = Lyrics,
            Tags = GetTags(),
            ProviderIds = new Dictionary<string, string>() { { ProviderNames.AnimeThemes, Id.ToString(CultureInfo.InvariantCulture) } }
        };

        if (song is not null)
        {
            result.Album = song.Title;
            result.Artists = song.Performances?.GetArtistsNames();
        }

        if (anime is not null)
        {
            result.ExternalId = anime.Slug;
            result.ProductionYear = anime.Year;
            result.Studios = anime.Studios?.GetStudiosNames();
            result.Overview = anime.Synopsis;
        }

        return result;
    }

    public RemoteSearchResult ToRemoteSearchResult()
    {
        var anime = AnimethemeEntries?.Nodes.FirstOrDefault()?.Animetheme?.Anime;

        return new RemoteSearchResult
        {
            Name = GetName(),
            ProductionYear = anime?.Year,
            ImageUrl = anime?.Images?.GetLargeImageLink(),
            SearchProviderName = ProviderNames.AnimeThemes,
            ProviderIds = new Dictionary<string, string>() { { ProviderNames.AnimeThemes, Id.ToString(CultureInfo.InvariantCulture) } }
        };
    }

    private string GetName()
    {
        var node = AnimethemeEntries?.Nodes.FirstOrDefault();
        var animetheme = node?.Animetheme;
        var anime = animetheme?.Anime;

        if (string.IsNullOrEmpty(anime?.Name))
        {
            return Filename;
        }

        var name = new StringBuilder();

        name.Append(anime?.Name);
        name.Append(" - ");
        name.Append(animetheme?.Slug);

        var version = node?.Version;
        if (version is not null)
        {
            name.AppendFormat(CultureInfo.InvariantCulture, "v{0}", version);
        }

        if (!string.IsNullOrEmpty(Tags))
        {
            name.Append(" - ");
            name.Append(Tags);
        }

        return name.ToString();
    }

    private string[] GetTags()
    {
        var result = new List<string>();

        var animetheme = AnimethemeEntries?.Nodes.FirstOrDefault()?.Animetheme;
        var anime = animetheme?.Anime;

        result.AddIfNotNull(anime?.Name);
        result.AddRangeIfNotNull(anime?.Animesynonyms?.Data.Select(x => x.Text));

        result.AddIfNotNull(anime?.Year.ToString(CultureInfo.InvariantCulture));
        result.AddIfNotNull(anime?.Season);
        if (anime?.Season is not null && anime?.Year is not null)
        {
            result.Add($"{anime.Season} {anime.Year}");
        }

        result.AddIfNotNull(animetheme?.Type);
        result.AddIfNotNull(animetheme?.Slug);

        if (!string.IsNullOrEmpty(Tags))
        {
            result.AddRangeIfNotNull(Tags.Split(","));
        }

        result.AddIfNotNull(Source);

        return result.ToArray();
    }
}
