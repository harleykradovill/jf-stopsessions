using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.StopSessions.Configuration;

/// <summary>
/// Plugin configuration.
/// </summary>
public class PluginConfiguration : BasePluginConfiguration
{
    /// <summary>
    /// Gets or sets the amount of seconds a device should be kept.
    /// </summary>
    public int PausedSeconds { get; set; } = 3600;
}
