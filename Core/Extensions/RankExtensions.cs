using System;

using BetterBeatSaber.Shared.Network.Enums;

namespace BetterBeatSaber.Core.Extensions; 

public static class RankExtensions {

    public static Rank ToRank(this RankModel.Rank rank) =>
        (Rank) Enum.Parse(typeof(Rank), rank.ToString());

}