using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.AnimeThemesMetadata.Providers.AnimeThemes;

public class AnimeThemesArtistImageProvider(ILogger<AnimeThemesArtistImageProvider> logger) : IRemoteImageProvider
{
    private readonly ILogger<AnimeThemesArtistImageProvider> _log = logger;
    private readonly ImageType[] supportedTypes = [ImageType.Primary];
    private readonly AnimeThemesApiClient _client = new AnimeThemesApiClient();

    public string Name => ProviderNames.AnimeThemes;

    public async Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken)
    {
        var httpClient = Plugin.Instance.GetHttpClient();
        return await httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
    }

    public async Task<IEnumerable<RemoteImageInfo>> GetImages(BaseItem item, CancellationToken cancellationToken)
    {
        var result = new List<RemoteImageInfo>();

        if (!item.TryGetProviderId(ProviderNames.AnimeThemes, out var stringId)
                || !string.IsNullOrEmpty(item.Name))
        {
            return result;
        }

        _log.LogDebug("Retrieving image link({Nmae})", item.Name);
        var artist = await _client.GetArtist(item.Name, cancellationToken).ConfigureAwait(false);
        if (artist is null)
        {
            return result;
        }

        var link = artist.GetBestImage();
        if (string.IsNullOrEmpty(link))
        {
            _log.LogDebug("Image not found({Nmae})", item.Name);
            return result;
        }

        result.Add(new RemoteImageInfo
        {
            ProviderName = Name,
            Type = ImageType.Primary,
            Url = link
        });

        return result;
    }

    public IEnumerable<ImageType> GetSupportedImages(BaseItem item) => supportedTypes;

    public bool Supports(BaseItem item) => item is Person;
}
