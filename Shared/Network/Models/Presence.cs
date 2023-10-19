using BetterBeatSaber.Shared.Enums;

using LiteNetLib.Utils;

namespace BetterBeatSaber.Shared.Models;

public interface IPresence : INetSerializable {
    
    public Status Status { get; }
    
    public interface IMap : IPresence {
        
        public Map Map { get; }
        public DifficultyMap Difficulty { get; }

    }
    
}

public static class Presence {

    public struct Offline : IPresence {

        public Status Status => Status.Offline;
        
        public void Serialize(NetDataWriter writer) {
        }

        public void Deserialize(NetDataReader reader) {
        }

    }

    public struct InMenu : IPresence {
        
        public Status Status => Status.InMenu;
        
        public void Serialize(NetDataWriter writer) {
        }

        public void Deserialize(NetDataReader reader) {
        }
        
    }
    
    public struct PlayingMap : IPresence.IMap {
        
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
    
    public struct PlayingTutorial : IPresence {
        
        public Status Status => Status.PlayingTutorial;
        
        public void Serialize(NetDataWriter writer) {
        }

        public void Deserialize(NetDataReader reader) {
        }

    }

    public struct WatchingReplay : IPresence.IMap {
        
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
    
    public struct Afk : IPresence {
        
        public Status Status => Status.Afk;
        
        public void Serialize(NetDataWriter writer) {
        }

        public void Deserialize(NetDataReader reader) {
        }

    }
    
}
