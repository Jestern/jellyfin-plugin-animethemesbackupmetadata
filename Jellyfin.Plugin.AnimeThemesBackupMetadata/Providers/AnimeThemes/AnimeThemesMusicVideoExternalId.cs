using System;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;

namespace Jellyfin.Plugin.AnimeThemesBackupMetadata.Providers.AnimeThemes;

public class AnimeThemesMusicVideoExternalId : IExternalId
{
    public string ProviderName => "AnimeThemes";

    public string Key => ProviderNames.AnimeThemes;

    public ExternalIdMediaType? Type => null;

    public string? UrlFormatString => "https://animethemes.moe/anime/{0}/";

    public bool Supports(IHasProviderIds item)
        => item is MusicVideo;
}
