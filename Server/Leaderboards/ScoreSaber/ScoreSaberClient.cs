using BetterBeatSaber.Server.Extensions;
using BetterBeatSaber.Server.Leaderboards.ScoreSaber.Interfaces;
using BetterBeatSaber.Server.Leaderboards.ScoreSaber.Models;
using BetterBeatSaber.Server.Models;
using BetterBeatSaber.Shared.Enums;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BetterBeatSaber.Server.Leaderboards.ScoreSaber; 

public sealed class ScoreSaberClient : BaseLeaderboardClient<ScoreSaberClient>, IScoreSaberClient {
    
    private const string BaseUrl = "https://scoresaber.com/api";
    
    private static readonly JsonSerializerSettings SerializerSettings = new() {
        ContractResolver = new DefaultContractResolver {
            NamingStrategy = new CamelCaseNamingStrategy()
        }
    };

    public ScoreSaberClient(HttpClient httpClient) : base(httpClient) { }
    
    public async Task<PlayerInformation?> GetPlayerInformation(string id, bool full = true) =>
        await HttpClient.GetJsonAsync<PlayerInformation>($"{BaseUrl}/player/{id}/{(full ? "full" : "basic")}", SerializerSettings);

    public async Task<uint?> GetPlayerCount(string? search = null, string? country = null) {

        var query = new Dictionary<string, string>();
        
        if(search != null)
            query.Add("search", search);
        
        if(country != null)
            query.Add("countries", country);
        
        return uint.TryParse(await HttpClient.GetStringAsync($"{BaseUrl}/players/count{query.BuildQueryString()}"), out var count) ? count : null;
        
    }

    public async Task<PlayerInformationList?> GetPlayers(string? search = null, int? page = null, bool? withMetadata = null, params string[] countries) {
        
        var query = new Dictionary<string, string>();
        
        if(search != null)
            query.Add("search", search);
        
        if(page != null)
            query.Add("page", page.Value.ToString());
        
        if(countries.Length > 0)
            query.Add("countries", string.Join(",", countries));
        
        if(withMetadata != null)
            query.Add("withMetadata", withMetadata.Value.ToString());
        
        return await HttpClient.GetJsonAsync<PlayerInformationList>($"{BaseUrl}/players{query.BuildQueryString()}", SerializerSettings);
        
    }

    public override async Task UpdatePlayer(Player player) {

        var playerInformation = await GetPlayerInformation(player.Id.ToString());
        if (playerInformation == null)
            return;

        if (playerInformation.Inactive) {
            if(player.Flags.HasFlag(PlayerFlag.HasScoreSaber))
                player.Flags &= ~PlayerFlag.HasScoreSaber;
            return;
        }
        
        player.ScoreSaberPp = playerInformation.Pp;
        player.ScoreSaberGlobalRank = playerInformation.Rank;
        player.ScoreSaberCountryRank = playerInformation.CountryRank;
        player.ScoreSaberCountry = playerInformation.Country;

        if (!player.Flags.HasFlag(PlayerFlag.HasScoreSaber))
            player.Flags |= PlayerFlag.HasScoreSaber;

    }

}