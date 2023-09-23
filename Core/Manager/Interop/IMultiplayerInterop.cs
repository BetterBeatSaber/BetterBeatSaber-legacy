using System;

using BetterBeatSaber.Shared.Enums;
using BetterBeatSaber.Shared.Network.Interfaces;

namespace BetterBeatSaber.Core.Manager.Interop;

public interface IMultiplayerInterop<T> : IInterop where T : ILobby {

    public MultiplayerService Service { get; }
    
    public event Action<T>? OnLobbyJoined;
    public event Action? OnLobbyLeft;

    public bool JoinLobby(T lobby);

}