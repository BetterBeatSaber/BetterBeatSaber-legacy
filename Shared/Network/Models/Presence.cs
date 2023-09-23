using BetterBeatSaber.Shared.Enums;
using BetterBeatSaber.Shared.Network.Interfaces;

using LiteNetLib.Utils;

namespace BetterBeatSaber.Shared.Models; 

public struct InMenuPresence : IPresence {

    public Status Status => Status.InMenu;
    
    public void Serialize(NetDataWriter writer) {
    }

    public void Deserialize(NetDataReader reader) {
    }

}
    
public struct PlayingTutorialPresence : IPresence {

    public Status Status => Status.PlayingTutorial;
        
    public void Serialize(NetDataWriter writer) {
    }

    public void Deserialize(NetDataReader reader) {
    }

}
    
public struct PlayingMapPresence : IMapPresence {

    public Status Status => Status.PlayingMap;
    
    public Map Map { get; set; }
    public DifficultyMap Difficulty { get; set; }

    public void Serialize(NetDataWriter writer) {
        writer.Put(Map);
        writer.Put(Difficulty);
    }

    public void Deserialize(NetDataReader reader) {
        Map = reader.Get<Map>();
        Difficulty = reader.Get<DifficultyMap>();
    }

}

public struct WatchingReplayPresence : IMapPresence {

    public Status Status => Status.WatchingReplay;
    
    public Map Map { get; set; }
    public DifficultyMap Difficulty { get; set; }
    public User User { get; set; }
    
    public void Serialize(NetDataWriter writer) {
        writer.Put(Map);
        writer.Put(Difficulty);
        writer.Put(User);
    }

    public void Deserialize(NetDataReader reader) {
        Map = reader.Get<Map>();
        Difficulty = reader.Get<DifficultyMap>();
        User = reader.Get<User>();
    }

}

public struct AfkPresence : IPresence {

    public Status Status => Status.Afk;
    
    public void Serialize(NetDataWriter writer) {
    }

    public void Deserialize(NetDataReader reader) {
    }

}

public struct OfflinePresence : IPresence {

    public Status Status => Status.Offline;
    
    public void Serialize(NetDataWriter writer) {
    }

    public void Deserialize(NetDataReader reader) {
    }

}