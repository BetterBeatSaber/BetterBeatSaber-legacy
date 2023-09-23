using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

using BetterBeatSaber.Core.Api;
using BetterBeatSaber.Core.Extensions;
using BetterBeatSaber.Core.Network;
using BetterBeatSaber.Shared.Models;
using BetterBeatSaber.Shared.Responses;

using Steamworks;

namespace BetterBeatSaber.Core.Manager; 

internal sealed class AuthManager : Manager<AuthManager> {

    internal string Token { get; private set; } = null!;

    public Player CurrentPlayer { get; private set; }
    
    internal async Task Authenticate() {
        
        #region Steam Ticket

        if (!SteamAPI.IsSteamRunning() || !SteamAPI.Init())
            throw new Exception("Steam not running or not initialized");
        
        var ticketRaw = new byte[1024];
        if (SteamUser.GetAuthSessionTicket(ticketRaw, ticketRaw.Length, out var ticketSize) == HAuthTicket.Invalid)
            throw new Exception("Failed to retrieve Steam ticket");
        
        var flag = false;
        for (uint index = 0; index < ticketSize; index++) {
            if (ticketRaw[(int) index] == 0)
                continue;
            flag = true;
            break;
        }
        
        var ticket = flag ? BitConverter.ToString(ticketRaw, 0, (int) ticketSize).Replace("-", "") : null;
        if (ticket == null || string.IsNullOrEmpty(ticket))
            throw new Exception("Steam ticket is invalid");
        
        #endregion

        var response = await ApiClient.Instance.PostRaw("/auth", new StringContent(ticket, Encoding.UTF8, "text/plain"));
        if (response == null)
            throw new AuthenticationException("Failed to authenticate");

        if (!response.IsSuccessStatusCode)
            throw new AuthenticationException($"Failed to authenticate: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
        
        var authResponse = await response.Content.ReadAsJsonAsync<AuthResponse>();
        if (authResponse == null)
            throw new AuthenticationException("Failed to authenticate");
        
        Token = authResponse.Token;
        
        ApiClient.Instance.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResponse.Token);

        CurrentPlayer = authResponse.Player;
        
        NetworkClient.Ip = authResponse.Ip;
        NetworkClient.Port = authResponse.Port;
        NetworkClient.Key = authResponse.Key;

    }
    
}