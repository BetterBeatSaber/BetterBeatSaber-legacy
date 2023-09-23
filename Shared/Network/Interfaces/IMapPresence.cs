using BetterBeatSaber.Shared.Models;

namespace BetterBeatSaber.Shared.Network.Interfaces; 

public interface IMapPresence : IPresence {

    public Map Map { get; set; }
    public DifficultyMap Difficulty { get; set; }

}