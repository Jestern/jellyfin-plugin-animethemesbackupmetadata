using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json.Serialization;
using Jellyfin.Extensions;
using Jellyfin.Plugin.AnimeThemesMetadata.Extensions;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Audio;

namespace Jellyfin.Plugin.AnimeThemesMetadata.Providers.AnimeThemes;

#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

public class RootDto
{
    [JsonPropertyName("data")]
    public RootDataDto? Data { get; set; }
}

public class RootDataDto
{
    [JsonPropertyName("videoPaginator")]
    public VideoPaginatorDto? VideoPaginator { get; set; }

    [JsonPropertyName("artistPaginator")]
    public ArtistPaginatorDto? ArtistPaginator { get; set; }
}

public class VideoPaginatorDto
{
    [JsonPropertyName("data")]
    public List<VideoDto> Data { get; set; }
}

public class ArtistPaginatorDto
{
    [JsonPropertyName("data")]
    public List<ArtistDto> Data { get; set; }
}

public class VideoDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("filename")]
    public string Filename { get; set; }

    [JsonPropertyName("lyrics")]
    public bool Lyrics { get; set; }

    [JsonPropertyName("tags")]
    public string? Tags { get; set; }

    [JsonPropertyName("source")]
    public string? Source { get; set; }

    [JsonPropertyName("animethemeentries")]
    public AnimethemeEntriesDto? AnimethemeEntries { get; set; }

    public MusicVideo ToMusicVideo()
    {
        var animetheme = AnimethemeEntries?.Nodes.FirstOrDefault()?.Animetheme;
        var anime = animetheme?.Anime;

        var result = new MusicVideo
        {
            Name = string.IsNullOrEmpty(anime?.Name) ? Filename : $"{anime?.Name} - {animetheme?.Slug} - {Tags}",
            ExternalId = anime?.Slug,
            HasSubtitles = Lyrics,
            Album = animetheme?.Song?.Title,
            Artists = animetheme?.Song?.Performances?.Data.Select(x => x.Artist?.Name).ToArray(),
            ProductionYear = anime?.Year,
            Studios = anime?.Studios?.Nodes?.Select(x => x.Name).ToArray(),
            Tags = GetTags(),
            Overview = anime?.Synopsis,
            ProviderIds = new Dictionary<string, string>() { { ProviderNames.AnimeThemes, Id.ToString(CultureInfo.InvariantCulture) } }
        };

        return result;
    }

    private string[] GetTags()
    {
        var result = new List<string>();

        var animetheme = AnimethemeEntries?.Nodes.FirstOrDefault()?.Animetheme;
        var anime = animetheme?.Anime;

        result.AddIfNotNull(anime?.Name);
        result.AddRangeIfNotNull(anime?.Animesynonyms?.Data.Select(x => x.Text));

        result.AddIfNotNull(anime?.Season);
        if (anime?.Season is not null && anime?.Year is not null)
        {
            result.Add($"{anime.Season} {anime.Year}");
        }

        result.AddIfNotNull(animetheme?.Type);
        result.AddIfNotNull(animetheme?.Slug);
        result.AddRangeIfNotNull(Tags?.Split(","));
        result.AddIfNotNull(Source);

        return result.ToArray();
    }
}

public class AnimethemeEntriesDto
{
    [JsonPropertyName("nodes")]
    public List<AnimethemeNodeDto> Nodes { get; set; }
}

public class AnimethemeNodeDto
{
    [JsonPropertyName("animetheme")]
    public AnimethemeDto? Animetheme { get; set; }
}

public class AnimethemeDto
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("slug")]
    public string? Slug { get; set; }

    [JsonPropertyName("anime")]
    public AnimeDto? Anime { get; set; }

    [JsonPropertyName("song")]
    public SongDto? Song { get; set; }
}

public class AnimeDto
{
    [JsonPropertyName("slug")]
    public string? Slug { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("year")]
    public int? Year { get; set; }

    [JsonPropertyName("season")]
    public string? Season { get; set; }

    [JsonPropertyName("synopsis")]
    public string? Synopsis { get; set; }

    [JsonPropertyName("animesynonyms")]
    public AnimeSynonymsDto? Animesynonyms { get; set; }

    [JsonPropertyName("studios")]
    public StudiosDto? Studios { get; set; }
}

public class AnimeSynonymsDto
{
    [JsonPropertyName("data")]
    public List<AnimeSynonymDto> Data { get; set; }
}

public class AnimeSynonymDto
{
    [JsonPropertyName("text")]
    public string? Text { get; set; }
}

public class StudiosDto
{
    [JsonPropertyName("nodes")]
    public List<StudioNodeDto> Nodes { get; set; }
}

public class StudioNodeDto
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}

public class SongDto
{
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("performances")]
    public PerformancesDto? Performances { get; set; }
}

public class PerformancesDto
{
    [JsonPropertyName("data")]
    public List<PerformanceDto> Data { get; set; }
}

public class PerformanceDto
{
    [JsonPropertyName("artist")]
    public ArtistDto? Artist { get; set; }
}

public class ArtistDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("slug")]
    public string? Slug { get; set; }

    [JsonPropertyName("images")]
    public ImagesDto? Images { get; set; }

    public MusicArtist ToMusicArtist()
    {
        return new MusicArtist
        {
            Name = Name,
            ProviderIds = new Dictionary<string, string>() { { ProviderNames.AnimeThemes, Id.ToString(CultureInfo.InvariantCulture) } }
        };
    }

    public string? GetBestImage()
    {
        var result = Images?.Nodes.FirstOrDefault(x => string.Equals(x.Facet, "LARGE_COVER", System.StringComparison.Ordinal))?.Link;
        if (string.IsNullOrEmpty(result))
        {
            result = Images?.Nodes.FirstOrDefault(x => string.Equals(x.Facet, "SMALL_COVER", System.StringComparison.Ordinal))?.Link;
        }

        return result;
    }
}

public class ImagesDto
{
    [JsonPropertyName("nodes")]
    public List<ImageNodeDto> Nodes { get; set; }
}

public class ImageNodeDto
{
    [JsonPropertyName("facet")]
    public string? Facet { get; set; }

    [JsonPropertyName("link")]
    public string? Link { get; set; }
}
