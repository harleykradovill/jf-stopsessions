using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Data.Queries;
using MediaBrowser.Controller.Devices;
using MediaBrowser.Controller.Session;
using MediaBrowser.Model.Globalization;
using MediaBrowser.Model.Session;
using MediaBrowser.Model.Tasks;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.StopSessions;

/// <summary>
/// Device cleaner task.
/// </summary>
public class StopSessionsTask : IScheduledTask, IConfigurableScheduledTask
{
    private readonly IDeviceManager _deviceManager;
    private readonly ISessionManager _sessionManager;
    private readonly ILocalizationManager _localizationManager;
    private readonly ILogger<StopSessionsTask> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="StopSessionsTask"/> class.
    /// </summary>
    /// <param name="sessionManager">Instance of the <see cref="ISessionManager"/> interface.</param>
    /// <param name="localizationManager">Instance of the <see cref="ILocalizationManager"/> interface.</param>
    /// <param name="deviceManager">Instance of the <see cref="IDeviceManager"/> interface.</param>
    /// <param name="logger">Instance of the <see cref="ILogger"/> interface. </param>
    public StopSessionsTask(
        ISessionManager sessionManager,
        ILocalizationManager localizationManager,
        IDeviceManager deviceManager,
        ILogger<StopSessionsTask> logger)
    {
        _sessionManager = sessionManager;
        _localizationManager = localizationManager;
        _deviceManager = deviceManager;
        _logger = logger;
    }

    /// <inheritdoc />
    public bool IsHidden => false;

    /// <inheritdoc />
    public bool IsEnabled => true;

    /// <inheritdoc />
    public bool IsLogged => true;

    /// <inheritdoc />
    public string Name => "Stop Paused Sessions";

    /// <inheritdoc />
    public string Key => "StopPausedSessions";

    /// <inheritdoc />
    public string Description => "Removes sessions older then the configured age.";

    /// <inheritdoc />
    public string Category => _localizationManager.GetLocalizedString("TasksMaintenanceCategory");

    /// <inheritdoc />
    public async Task ExecuteAsync(IProgress<double> progress, CancellationToken cancellationToken)
    {
        if (StopSessionsPlugin.Instance?.Configuration == null)
        {
            _logger.LogError("[### Stop Sessions] Plugin configuration is missing. Cannot proceed.");
            return;
        }

        ArgumentNullException.ThrowIfNull(StopSessionsPlugin.Instance?.Configuration);

        var config = StopSessionsPlugin.Instance.Configuration;

        if (!config.Enabled)
        {
            _logger.LogInformation("[### Stop Sessions] Plugin is disabled. Skipping task execution.");
            return;
        }

        var pausedValue = config.PausedValue;
        var pausedUnit = config.PausedUnit;

        double threshholdSeconds = pausedUnit switch // Convert to seconds
        {
            "minutes" => pausedValue * 60,
            "hours" => pausedValue * 3600,
            _ => pausedValue
        };

        _logger.LogInformation("[### Stop Sessions] Threshold seconds set to: {ThreshholdSeconds} seconds", threshholdSeconds);

        var sessions = _sessionManager.GetSessions(Guid.Empty, null, null, null, true);

        foreach (var session in sessions)
        {
            if (session.PlayState != null && session.NowPlayingItem != null && session.LastPausedDate != null)
            {
                var pausedDuration = DateTime.UtcNow - session.LastPausedDate.Value;
                if (pausedDuration.TotalSeconds > threshholdSeconds)
                {
                    var playstateRequest = new PlaystateRequest
                    {
                        Command = PlaystateCommand.Stop
                    };

                    await _sessionManager.SendPlaystateCommand(
                        controllingSessionId: null,
                        sessionId: session.Id,
                        command: playstateRequest,
                        cancellationToken: cancellationToken).ConfigureAwait(false);

                    _logger.LogInformation("[### Stop Sessions] Sent stop command to session {SessionId}.", session.Id);
                }
            }
        }
    }

    /// <inheritdoc />
    public IEnumerable<TaskTriggerInfo> GetDefaultTriggers()
    {
        yield return new TaskTriggerInfo
        {
            Type = TaskTriggerInfo.TriggerInterval,
            IntervalTicks = TimeSpan.FromMinutes(2).Ticks
        };
    }
}
