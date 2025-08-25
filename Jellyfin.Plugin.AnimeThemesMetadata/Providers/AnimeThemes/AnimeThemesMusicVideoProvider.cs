using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.AnimeThemesMetadata.Providers.AnimeThemes;

public class AnimeThemesMusicVideoProvider(ILogger<AnimeThemesMusicVideoProvider> logger) : IRemoteMetadataProvider<MusicVideo, MusicVideoInfo>, IHasOrder
{
    private readonly ILogger<AnimeThemesMusicVideoProvider> _log = logger;
    private readonly AnimeThemesApiClient _client = new AnimeThemesApiClient();

    public string Name => ProviderNames.AnimeThemes;

    public int Order => 1;

    public async Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken)
    {
        var httpClient = Plugin.Instance.GetHttpClient();

        return await httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
    }

    public async Task<MetadataResult<MusicVideo>> GetMetadata(MusicVideoInfo info, CancellationToken cancellationToken)
    {
        var result = new MetadataResult<MusicVideo>();
        var filename = Path.GetFileNameWithoutExtension(info.Path);

        _log.LogInformation("Animethemes searching metadata({Name})", filename);
        var video = await _client.GetMusicVideo(filename, cancellationToken)
            .ConfigureAwait(false);

        if (video is null)
        {
            return result;
        }

        result.HasMetadata = true;
        result.Item = video.ToMusicVideo();
        result.Provider = ProviderNames.AnimeThemes;

        return result;
    }

    public Task<IEnumerable<RemoteSearchResult>> GetSearchResults(MusicVideoInfo searchInfo, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
