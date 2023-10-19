using BetterBeatSaber.Core.Manager;
using BetterBeatSaber.Shared.Models;

namespace BetterBeatSaber.Core.Extensions; 

public static class PlayerExtensions {

    public static IPresence? GetPresence(this Player player) =>
        FriendManager.Instance.GetFriendPresence(player);

    public static Lobby? GetLobby(this Player player) =>
        FriendManager.Instance.GetFriendLobby(player);

}