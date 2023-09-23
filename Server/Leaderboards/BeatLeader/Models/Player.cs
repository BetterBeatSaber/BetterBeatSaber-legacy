namespace BetterBeatSaber.Server.Leaderboards.BeatLeader.Models; 

#pragma warning disable CS8618

public class Player {

    public int MapperId { get; set; }
    public bool Banned { get; set; }
    public bool Inactive { get; set; }
    public string? BanDescription { get; set; }
    public string ExternalProfileUrl { get; set; }
    public IEnumerable<object>? History { get; set; }
    public IEnumerable<Badge>? Badges { get; set; }
    public IEnumerable<object>? PinnedScores { get; set; }
    public IEnumerable<object>? Changes { get; set; }
    public float AccPp { get; set; }
    public float PassPp { get; set; }
    public float TechPp { get; set; }
    public object? ScoreStats { get; set; }
    public float LastWeekPp { get; set; }
    public uint LastWeekRank { get; set; }
    public uint LastWeekCountryRank { get; set; }
    public IEnumerable<Event>? EventsParticipating { get; set; }
    public string Id { get; set; }
    public string Name { get; set; }
    public string Platform { get; set; }
    public string Avatar { get; set; }
    public string Country { get; set; }
    public bool Bot { get; set; }
    public float Pp { get; set; }
    public uint Rank { get; set; }
    public uint CountryRank { get; set; }
    public string Role { get; set; }
    public IEnumerable<Social>? Socials { get; set; }
    public PatreonFeature? PatreonFeatures { get; set; }
    public ProfileSettings ProfileSettings { get; set; }
    public IEnumerable<Clan>? Clans { get; set; }

    public class Badge {

        public uint Id { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Link { get; set; }

    }
    
    public class Event {

        public uint Id { get; set; }
        public uint EventId { get; set; }
        public string Name { get; set; }
        public string PlayerId { get; set; }
        public string Country { get; set; }
        public uint Rank { get; set; }
        public uint CountryRank { get; set; }
        public float Pp { get; set; }

    }
    
    public class Social {

        public uint Id { get; set; }
        public string Service { get; set; }
        public string Link { get; set; }
        public string User { get; set; }
        public string UserId { get; set; }
        public string PlayerId { get; set; }

    }
    
    public class PatreonFeature {

        public uint Id { get; set; }
        public string Bio { get; set; }
        public string Message { get; set; }
        public string LeftSaberColor { get; set; }
        public string RightSaberColor { get; set; }

    }
    
    public class Clan {

        public uint Id { get; set; }
        public string Tag { get; set; }
        public string Color { get; set; }

    }
    
}