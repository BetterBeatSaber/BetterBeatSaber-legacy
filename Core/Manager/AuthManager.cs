using System;
using System.Collections;
using System.Net.Http.Headers;
using System.Text;

using BetterBeatSaber.Core.Api;
using BetterBeatSaber.Core.Network;
using BetterBeatSaber.Core.Utilities;
using BetterBeatSaber.Shared.Models;
using BetterBeatSaber.Shared.Responses;

using UnityEngine;

namespace BetterBeatSaber.Core.Manager; 

internal sealed class AuthManager : Manager<AuthManager> {

    internal string? Token { get; private set; }

    public Player? CurrentPlayer { get; private set; }

    public bool IsAuthenticated => Token != null;

    public event Action<string>? OnAuthenticationFailed;
    public event Action? OnAuthenticated;

    public override void Init() =>
        ThreadDispatcher.Enqueue(Authenticate());

    private IEnumerator Authenticate() {
        
        yield return new WaitUntil(() => SteamManager.Initialized);

        var ticketTask = new SteamPlatformUserModel().GetUserAuthToken();
        yield return new WaitUntil(() => ticketTask.IsCompleted);

        var request = new ApiRequest<AuthResponse>("/auth", "POST") {
            BodyRaw = Encoding.UTF8.GetBytes(ticketTask.Result.token ?? string.Empty)
        };
        
        yield return request.Send();

        if (request.Failed) {
            OnAuthenticationFailed?.Invoke(request.Error);
            Logger.Warn("Authentication failed: {0}", request.Error);
            yield break;
        }

        if (request.Response == null) {
            OnAuthenticationFailed?.Invoke("Failed to parse response");
            Logger.Warn("Failed to parse response");
            yield break;
        }
        
        Token = request.Response.Token;
        
        ApiClient.Instance.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", request.Response.Token);

        CurrentPlayer = request.Response.Player;
        
        NetworkClient.Ip = request.Response.Ip;
        NetworkClient.Port = request.Response.Port;
        NetworkClient.Key = request.Response.Key;
        
        OnAuthenticated?.Invoke();
        
        Logger.Info("Authenticated as {0}", request.Response.Player.Name);
        
    }
    
}