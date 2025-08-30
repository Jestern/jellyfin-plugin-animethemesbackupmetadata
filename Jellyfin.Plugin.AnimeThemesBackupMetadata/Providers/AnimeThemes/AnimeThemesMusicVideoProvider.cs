using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.AnimeThemesBackupMetadata.Providers.AnimeThemes.Model;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.AnimeThemesBackupMetadata.Providers.AnimeThemes;

public class AnimeThemesMusicVideoProvider(ILogger<AnimeThemesMusicVideoProvider> logger) : IRemoteMetadataProvider<MusicVideo, MusicVideoInfo>, IHasOrder
{
    private readonly ILogger<AnimeThemesMusicVideoProvider> _log = logger;
    private readonly AnimeThemesApiClient _client = new AnimeThemesApiClient(logger);

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

        _log.LogInformation("Animethemes searching metadata({Path})", info.Path);

        info.ProviderIds.TryGetValue(ProviderNames.AnimeThemes, out var atId);

        var video = await (!string.IsNullOrEmpty(atId) ?
            GetVideoByIdAsync(atId, cancellationToken) :
            _client.GetMusicVideoByFilename(filename, cancellationToken)
        ).ConfigureAwait(false);

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
            _log.LogInformation("Animethemes searching theme with id: {Id}", atId);

            var video = await GetVideoByIdAsync(atId, cancellationToken).ConfigureAwait(false);
            if (video is not null)
            {
                results.Add(video.ToRemoteSearchResult());
            }
        }
        else
        {
            _log.LogInformation("Animethemes searching themes for {Path}", searchInfo.Name);

            var animes = await _client.SearchByName(searchInfo.Name, cancellationToken).ConfigureAwait(false);
            animes.ForEach(a => results.AddRange(a.ToRemoteSearchResults()));
        }

        return results;
    }

    private async Task<VideoDto> GetVideoByIdAsync(string atId, CancellationToken cancellationToken)
    {
        var validId = int.TryParse(atId, CultureInfo.InvariantCulture, out var atIdParsed);
        if (!validId)
        {
            _log.LogError("Animethemes id is not a number ({Id})", atId);

            throw new ArgumentException("Id should be a number.");
        }

        return await _client.GetMusicVideoById(int.Parse(atId, CultureInfo.InvariantCulture), cancellationToken).ConfigureAwait(false);
    }
}
