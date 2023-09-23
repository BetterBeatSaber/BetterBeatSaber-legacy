namespace BetterBeatSaber.Server.Leaderboards.ScoreSaber.Models; 

#pragma warning disable CS8618

public class PlayerInformationList {

    public IEnumerable<PlayerInformation> Players { get; set; }
    public PlayerInformationListMetadata Metadata { get; set; }
    
    public class PlayerInformationListMetadata {

        public int Total { get; set; }
        public int Page { get; set; }
        public int ItemsPerPage { get; set; }

    }

}