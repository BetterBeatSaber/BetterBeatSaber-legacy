using System;

namespace BetterBeatSaber.Core.Zenject; 

// ReSharper disable UnusedMember.Global

[Flags]
public enum Location : ushort {

    None = 0,
    App = 1, // Installed in app context
    Menu = 2, // Installed in menu context
    StandardPlayer = 4,
    CampaignPlayer = 8,
    MultiPlayer = 16,
    Player = StandardPlayer | CampaignPlayer | MultiPlayer,
    Tutorial = 32,
    GameCore = 64,
    MultiplayerCore = 128,
    SinglePlayer = StandardPlayer | CampaignPlayer | Tutorial,
    ConnectedPlayer = 256,
    AlwaysMultiPlayer = 512,
    InactiveMultiPlayer = 1024

}

public static class LocationExtensions {

    public static bool IsPlayer(this Location location) {
        return (location & Location.Player) != 0 || (location & Location.StandardPlayer) != 0 || (location & Location.CampaignPlayer) != 0 || (location & Location.MultiPlayer) != 0;
    }

}