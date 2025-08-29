using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.AnimeThemesBackupMetadata.Configuration;

/// <summary>
/// Plugin configuration.
/// </summary>
public class PluginConfiguration : BasePluginConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PluginConfiguration"/> class.
    /// </summary>
    public PluginConfiguration()
    {
        // set default options here
        AnimeThemesGraphqlUrl = "https://graphql.animethemes.moe/";
        Delay = 120;
        MaxRetryAttemps = 3;
    }

    /// <summary>
    /// Gets or sets a the url to AnimeThemes setting.
    /// </summary>
    public string AnimeThemesGraphqlUrl { get; set; }

    /// <summary>
    /// Gets or sets default delay between calls to the api.
    /// </summary>
    public int Delay { get; set; }

    /// <summary>
    /// Gets or sets the max number of attempts to get metadata.
    /// </summary>
    public int MaxRetryAttemps { get; set; }
}
