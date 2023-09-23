using System;

using BetterBeatSaber.Shared.Models;

namespace BetterBeatSaber.Core.Manager.Interop; 

public interface IReplayInterop : IInterop {

    public event Action<IDifficultyBeatmap, Replay>? OnReplayStarted;
    public event Action<IDifficultyBeatmap, Replay>? OnReplayEnded;

}