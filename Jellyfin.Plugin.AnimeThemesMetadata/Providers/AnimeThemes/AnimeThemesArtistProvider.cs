using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Audio;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.AnimeThemesMetadata.Providers.AnimeThemes;

public class AnimeThemesArtistProvider(ILogger<AnimeThemesArtistProvider> logger) : IRemoteMetadataProvider<MusicArtist, ArtistInfo>, IHasOrder
{
    private readonly ILogger<AnimeThemesArtistProvider> _log = logger;
    private readonly AnimeThemesApiClient _client = new AnimeThemesApiClient();

    public string Name => ProviderNames.AnimeThemes;

    public int Order => 1;

    public async Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken)
    {
        var httpClient = Plugin.Instance.GetHttpClient();
        return await httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
    }

    public async Task<MetadataResult<MusicArtist>> GetMetadata(ArtistInfo info, CancellationToken cancellationToken)
    {
        var result = new MetadataResult<MusicArtist>();

        if (!info.TryGetProviderId(ProviderNames.AnimeThemes, out var stringId)
            || !string.IsNullOrEmpty(info.Name))
        {
            return result;
        }

        _log.LogDebug("Getting artist information({Nmae})", info.Name);
        var artist = await _client.GetArtist(info.Name, cancellationToken).ConfigureAwait(false);
        if (artist is null)
        {
            return result;
        }

        result.Item = artist.ToMusicArtist();
        result.HasMetadata = true;

        return result;
    }

    public Task<IEnumerable<RemoteSearchResult>> GetSearchResults(ArtistInfo searchInfo, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
