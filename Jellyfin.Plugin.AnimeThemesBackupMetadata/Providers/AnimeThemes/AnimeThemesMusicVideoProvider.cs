using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.AnimeThemesBackupMetadata.Providers.AnimeThemes.Model;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Providers;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;

namespace Jellyfin.Plugin.AnimeThemesBackupMetadata.Providers.AnimeThemes;

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
        info.ProviderIds.TryGetValue(ProviderNames.AnimeThemes, out var atId);

        VideoDto? video = null;
        if (!string.IsNullOrEmpty(atId))
        {
            video = await _client.GetMusicVideoById(atId, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            video = await _client.GetMusicVideoByFilename(filename, cancellationToken).ConfigureAwait(false);
        }

        if (video is null)
        {
            return result;
        }

        result.HasMetadata = true;
        result.Item = video.ToMusicVideo();
        result.Provider = ProviderNames.AnimeThemes;

        return result;
    }

    public async Task<IEnumerable<RemoteSearchResult>> GetSearchResults(MusicVideoInfo searchInfo, CancellationToken cancellationToken)
    {
        var results = new List<RemoteSearchResult>();

        searchInfo.ProviderIds.TryGetValue(ProviderNames.AnimeThemes, out var atId);
        if (!string.IsNullOrEmpty(atId))
        {
            var video = await _client.GetMusicVideoById(atId, cancellationToken).ConfigureAwait(false);
            if (video is not null)
            {
                results.Add(video.ToRemoteSearchResult());
            }
        }
        else
        {
            var animes = await _client.SearchByName(searchInfo.Name, cancellationToken).ConfigureAwait(false);
            animes.ForEach(a => results.AddRange(a.ToRemoteSearchResults()));
        }

        return results;
    }
}
