using System;
using System.Reflection;

using BetterBeatSaber.Core.Extensions;
using BetterBeatSaber.Shared.Models;

using IPA.Loader;

namespace BetterBeatSaber.Core.Manager.Interop.Interops; 

public sealed class BeatLeaderInterop : IReplayInterop {

    public event Action<IDifficultyBeatmap, Replay>? OnReplayStarted;
    public event Action<IDifficultyBeatmap, Replay>? OnReplayEnded;

    private Assembly? _assembly;

    #region Init & Exit
    
    public void Init() {
        _assembly = PluginManager.GetPluginFromId("BeatLeader")?.Assembly;
        AddEventHandlers();
    }

    public void Exit() {
        RemoveEventHandlers();
        _assembly = null;
    }

    #endregion

    private void AddEventHandlers() {

        _assembly?.GetType("BeatLeader.Replayer.ReplayerLauncher").AddEventHandler("ReplayWasStartedEvent", nameof(HandleReplayWasStartedEvent), this);
        
    }

    private void RemoveEventHandlers() {
        
    }

    private void HandleReplayWasStartedEvent(object f) {
        
    }

    public bool ShouldRun() => PluginManager.GetPluginFromId("BeatLeader") != null;

}