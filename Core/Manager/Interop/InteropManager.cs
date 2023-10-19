using System;
using System.Collections.Generic;

using BetterBeatSaber.Core.Extensions;
using BetterBeatSaber.Core.Manager.Interop.Interops;
using BetterBeatSaber.Shared.Models;

namespace BetterBeatSaber.Core.Manager.Interop; 

public sealed class InteropManager : Manager<InteropManager> {

    private static readonly List<Type> InteropTypes = new() {
        typeof(BeatLeaderInterop)
    };

    private readonly List<IInterop> _interops = new();

    public event Action<IDifficultyBeatmap, Replay>? OnReplayStarted;
    public event Action<IDifficultyBeatmap, Replay>? OnReplayEnded;

    #region Init & Exit

    public override void Init() {
        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var type in InteropTypes) {

            var interop = type.Construct<IInterop>();
            if(interop == null)
                continue;

            if (!interop.ShouldRun())
                continue;
            
            _interops.Add(interop);

            switch (interop) {
                case IReplayInterop replayInterop:
                    replayInterop.OnReplayStarted += OnReplayStarted;
                    replayInterop.OnReplayEnded += OnReplayEnded;
                    break;
            }
            
            interop.Init();
            
            Logger.Info($"Ran {type.Name}");

        }
    }

    public override void Exit() {
        foreach (var interop in _interops)
            interop.Exit();
    }

    #endregion
    
}