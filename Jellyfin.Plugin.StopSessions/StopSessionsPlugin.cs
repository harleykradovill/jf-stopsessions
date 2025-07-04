﻿using System;
using System.Collections.Generic;
using Jellyfin.Plugin.StopSessions.Configuration;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;

namespace Jellyfin.Plugin.StopSessions;

/// <summary>
/// Plugin entrypoint.
/// </summary>
public class StopSessionsPlugin : BasePlugin<PluginConfiguration>, IHasWebPages
{
    private readonly Guid _id = new("28a6125c-cb57-4e8b-b031-421ac55cfa91");

    /// <summary>
    /// Initializes a new instance of the <see cref="StopSessionsPlugin"/> class.
    /// </summary>
    /// <param name="applicationPaths">Instance of the <see cref="IApplicationPaths"/> interface.</param>
    /// <param name="xmlSerializer">Instance of the <see cref="IXmlSerializer"/> interface.</param>
    public StopSessionsPlugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer)
        : base(applicationPaths, xmlSerializer)
    {
        Instance = this;
    }

    /// <summary>
    /// Gets the current plugin instance.
    /// </summary>
    public static StopSessionsPlugin? Instance { get; private set; }

    /// <inheritdoc />
    public override Guid Id => _id;

    /// <inheritdoc />
    public override string Name => "Stop Sessions";

    /// <inheritdoc />
    public override string Description => "Stops paused sessions.";

    /// <inheritdoc />
    public IEnumerable<PluginPageInfo> GetPages()
    {
        yield return new PluginPageInfo
        {
            Name = Name,
            EmbeddedResourcePath = GetType().Namespace + ".Configuration.config.html"
        };
    }
}
