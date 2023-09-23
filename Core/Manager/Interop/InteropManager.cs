using System;
using System.Collections.Generic;
using System.Linq;

using BetterBeatSaber.Core.Extensions;
using BetterBeatSaber.Core.Manager.Interop.Interops;
using BetterBeatSaber.Shared.Models;
using BetterBeatSaber.Shared.Network.Interfaces;

namespace BetterBeatSaber.Core.Manager.Interop; 

public sealed class InteropManager : Manager<InteropManager> {

    private static readonly List<Type> InteropTypes = new() {
    };

    private readonly List<IInterop> _interops = new();

    #region Multiplayer

    public event Action<ILobby>? OnLobbyJoined;
    public event Action? OnLobbyLeft;

    public bool JoinLobby(ILobby lobby) {
        var interop = (IMultiplayerInterop<ILobby>?) _interops.FirstOrDefault(interop => interop is IMultiplayerInterop<ILobby> multiplayerInterop && multiplayerInterop.Service == lobby.Service);
        return interop != null && interop.JoinLobby(lobby);
    }
    
    #endregion

    #region Replay

    public event Action<IDifficultyBeatmap, Replay>? OnReplayStarted;
    public event Action<IDifficultyBeatmap, Replay>? OnReplayEnded;

    #endregion
    
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
                case IMultiplayerInterop<ILobby> multiplayerInterop:
                    multiplayerInterop.OnLobbyJoined += OnLobbyJoined;
                    multiplayerInterop.OnLobbyLeft += OnLobbyLeft;
                    break;
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