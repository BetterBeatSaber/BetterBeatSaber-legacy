using BetterBeatSaber.Shared.Enums;
using BetterBeatSaber.Shared.Models;

namespace BetterBeatSaber.Core.Extensions; 

public static class PresenceExtensions {

    public static string GetStatusText(this IPresence? presence, Lobby? lobby = null) {
        if (presence == null)
            return "Offline";
        return (lobby != null && presence.Status != Status.Offline ? "In Lobby | " : string.Empty) + presence.Status switch {
            Status.Afk => "AFK",
            Status.InMenu => "In Menu",
            Status.PlayingMap => "Playing a Map",
            Status.PlayingTutorial => "Playing the Tutorial",
            Status.WatchingReplay => "Watching a Replay",
            _ => "Offline"
        };
    }

}