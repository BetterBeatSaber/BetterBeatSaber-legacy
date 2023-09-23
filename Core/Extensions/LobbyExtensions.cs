using BetterBeatSaber.Core.Manager.Interop;
using BetterBeatSaber.Shared.Network.Interfaces;

namespace BetterBeatSaber.Core.Extensions; 

public static class LobbyExtensions {

    public static bool Join(this ILobby lobby) =>
        InteropManager.Instance.JoinLobby(lobby);

}