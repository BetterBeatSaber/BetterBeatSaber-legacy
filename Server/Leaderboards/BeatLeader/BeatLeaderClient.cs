using BetterBeatSaber.Server.Extensions;
using BetterBeatSaber.Server.Leaderboards.BeatLeader.Interfaces;
using BetterBeatSaber.Server.Leaderboards.BeatLeader.Models;
using BetterBeatSaber.Shared.Enums;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using Player = BetterBeatSaber.Server.Leaderboards.BeatLeader.Models.Player;

namespace BetterBeatSaber.Server.Leaderboards.BeatLeader; 

public sealed class BeatLeaderClient : BaseLeaderboardClient<BeatLeaderClient>, IBeatLeaderClient {

    private const string BaseUrl = "https://api.beatleader.xyz";
    
    private static readonly JsonSerializerSettings SerializerSettings = new() {
        ContractResolver = new DefaultContractResolver {
            NamingStrategy = new CamelCaseNamingStrategy()
        }
    };
    
    public BeatLeaderClient(HttpClient httpClient) : base(httpClient) { }
    
    public async Task<Player?> GetPlayer(string id, bool stats = false, bool keepOriginalId = false) =>
        await HttpClient.GetJsonAsync<Player>($"{BaseUrl}/player/{id}?stats={stats}&keepOriginalId={keepOriginalId}", SerializerSettings);

    public async Task<PlayerList?> GetPlayerList(string? search = null,
                                                 string? sortBy = "pp",
                                                 uint? page = 1,
                                                 uint? count = 50,
                                                 byte? order = 0,
                                                 IEnumerable<string>? countries = null,
                                                 string? mapsType = "ranked",
                                                 string? ppType = "general",
                                                 bool? friends = false,
                                                 string? ppRange = null,
                                                 string? scoreRange = null,
                                                 string? platform = null,
                                                 string? role = null,
                                                 string? hmd = null,
                                                 IEnumerable<string>? clans = null,
                                                 int? activityPeriod = null,
                                                 bool? banned = null) {

        var query = new Dictionary<string, string>();
        
        if(search != null)
            query.Add("search", search);
        
        if(sortBy != null)
            query.Add("sortBy", sortBy);
        
        if(page != null)
            query.Add("page", page.Value.ToString());
        
        if(count != null)
            query.Add("count", count.Value.ToString());

        if(order != null)
            query.Add("order", order.Value.ToString());
        
        if(countries != null)
            query.Add("countries", string.Join(",", countries));
        
        if(mapsType != null)
            query.Add("mapsType", mapsType);
        
        if(ppType != null)
            query.Add("ppType", ppType);
        
        if(friends != null)
            query.Add("friends", friends.Value.ToString());
        
        if(ppRange != null)
            query.Add("pp_range", ppRange);
        
        if(scoreRange != null)
            query.Add("score_range", scoreRange);
        
        if(platform != null)
            query.Add("platform", platform);
        
        if(role != null)
            query.Add("role", role);
        
        if(hmd != null)
            query.Add("hmd", hmd);
        
        if(clans != null)
            query.Add("clans", string.Join(",", clans));
        
        if(activityPeriod != null)
            query.Add("activityPeriod", activityPeriod.Value.ToString());
        
        if(banned != null)
            query.Add("banned", banned.Value.ToString());

        return await HttpClient.GetJsonAsync<PlayerList>($"{BaseUrl}/players{query.BuildQueryString()}");

    }

    public override async Task UpdatePlayer(Server.Models.Player player) {
        
        var playerInformation = await GetPlayer(player.Id.ToString());
        if (playerInformation == null)
            return;
        
        player.BeatLeaderPp = playerInformation.Pp;
        player.BeatLeaderGlobalRank = playerInformation.Rank;
        player.BeatLeaderCountryRank = playerInformation.CountryRank;
        player.BeatLeaderCountry = playerInformation.Country;
        
        if (!player.Flags.HasFlag(PlayerFlag.HasBeatLeader))
            player.Flags |= PlayerFlag.HasBeatLeader;
        
    }

}