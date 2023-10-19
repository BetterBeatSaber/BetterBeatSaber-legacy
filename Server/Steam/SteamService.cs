using System.Runtime.CompilerServices.Extensions;

using BetterBeatSaber.Server.Extensions;
using BetterBeatSaber.Server.Steam.Models;

namespace BetterBeatSaber.Server.Steam; 

public sealed class SteamService : ISteamService {

    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public SteamService(IHttpClientFactory httpClientFactory, IConfiguration configuration) {
        _httpClient = httpClientFactory.CreateClient(nameof(SteamService));
        _httpClient.BaseAddress = new Uri("https://api.steampowered.com/");
        _apiKey = configuration.GetValue<string>("SteamApiKey")!;
    }
    
    public async Task<(AuthResponseParams?, AuthResponseError?)> Authenticate(uint appId, string ticket) {
        
        var response = await _httpClient.GetJsonAsync<AuthResponse>("ISteamUserAuth/AuthenticateUserTicket/v0001/" + new Dictionary<string, string> {
            { "key", _apiKey },
            { "appid", appId.ToString() },
            { "ticket", ticket }
        }.BuildQueryString());

        return response != null ? (response.Response.Params, response.Response.Error) : (null, null);

    }

}