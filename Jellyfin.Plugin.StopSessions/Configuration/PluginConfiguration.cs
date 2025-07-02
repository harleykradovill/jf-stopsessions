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
    public int PausedValue { get; set; } = 3600;

    /// <summary>
    /// Gets or sets the unit.
    /// </summary>
    public string PausedUnit { get; set; } = "Seconds";

    /// <summary>
    /// Gets or sets a value indicating whether plugin is enabled or not.
    /// </summary>
    public bool Enabled { get; set; } = false;
}
