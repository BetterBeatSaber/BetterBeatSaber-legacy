using System;

using BetterBeatSaber.Shared.Models;

using IPA.Loader;

namespace BetterBeatSaber.Core.Manager.Interop.Interops; 

public sealed class BeatLeaderInterop : IReplayInterop {

    public event Action<IDifficultyBeatmap, Replay>? OnReplayStarted;
    public event Action<IDifficultyBeatmap, Replay>? OnReplayEnded;
    
    public void Init() {
        
    }

    public void Exit() {
        
    }

    public bool ShouldRun() => PluginManager.GetPluginFromId("BeatLeader") != null;

}