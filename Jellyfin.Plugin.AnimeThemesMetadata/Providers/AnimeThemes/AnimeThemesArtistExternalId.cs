using System;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;

namespace Jellyfin.Plugin.AnimeThemesMetadata.Providers.AnimeThemes;

public class AnimeThemesArtistExternalId : IExternalId
{
    public string ProviderName => "AnimeThemes";

    public string Key => ProviderNames.AnimeThemes;

    public ExternalIdMediaType? Type => ExternalIdMediaType.Artist;

    public string? UrlFormatString => "https://animethemes.moe/artist/{0}/";

    public bool Supports(IHasProviderIds item)
        => item is Person;
}
