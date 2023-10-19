using System.Runtime.CompilerServices.Extensions;

using BetterBeatSaber.Server.Extensions;
using BetterBeatSaber.Shared.Enums;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BetterBeatSaber.Server.Integrations; 

public abstract class Integration {

    private readonly IServiceScopeFactory _scopeFactory;
    
    public abstract IntegrationType Type { get; }
    public abstract string AuthorizeUrl { get; }
    public abstract string TokenUrl { get; }
    public abstract IEnumerable<string> Scopes { get; }
    
    public string ClientId { get; private set; }
    public string ClientSecret { get; private set; }
    public string RedirectUri { get; private set; }
    
    protected Integration(IServiceScopeFactory scopeFactory) {

        _scopeFactory = scopeFactory;
        
        using var scope = scopeFactory.CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        
        var className = GetType().Name;
        var name = className[..^11];
        
        ClientId = configuration.GetValue<string>($"{name}:ClientId")!;
        ClientSecret = configuration.GetValue<string>($"{name}:ClientSecret")!;
        RedirectUri = configuration.GetValue<string>($"{name}:RedirectUri")!;

    }

    public string GetAuthorizationUrl(string? state = null) {
        
        var query = new Dictionary<string, string> {
            { "client_id", ClientId },
            { "redirect_uri", RedirectUri },
            { "response_type", "code" },
            { "scope", string.Join(" ", Scopes) }
        };
        
        if(state != null)
            query.Add("state", state);
        
        return $"{AuthorizeUrl}{query.BuildQueryString()}";
        
    }

    public async Task<TokenResponse?> GetToken(string code) {

        using var scope = _scopeFactory.CreateScope();
        using var httpClient = scope.ServiceProvider.GetService<HttpClient>()!;
        
        var response = await httpClient.SendAsync(new HttpRequestMessage {
            RequestUri = new Uri(TokenUrl),
            Method = HttpMethod.Post,
            Content = new FormUrlEncodedContent(new [] {
                new KeyValuePair<string, string>("client_id", ClientId),
                new KeyValuePair<string, string>("client_secret", ClientSecret),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("redirect_uri", RedirectUri)
            })
        });

        return await response.Content.ReadAsJsonAsync<TokenResponse>(new JsonSerializerSettings {
            ContractResolver = new DefaultContractResolver {
                NamingStrategy = new SnakeCaseNamingStrategy()
            }
        });
        
    }
    
    public async Task<TokenResponse?> RefreshToken(string refreshToken) {

        using var scope = _scopeFactory.CreateScope();
        using var httpClient = scope.ServiceProvider.GetService<HttpClient>()!;
        
        var response = await httpClient.SendAsync(new HttpRequestMessage {
            RequestUri = new Uri(TokenUrl),
            Method = HttpMethod.Post,
            Content = new FormUrlEncodedContent(new [] {
                new KeyValuePair<string, string>("client_id", ClientId),
                new KeyValuePair<string, string>("client_secret", ClientSecret),
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("refresh_token", refreshToken)
            })
        });

        return await response.Content.ReadAsJsonAsync<TokenResponse>(new JsonSerializerSettings {
            ContractResolver = new DefaultContractResolver {
                NamingStrategy = new SnakeCaseNamingStrategy()
            }
        });
        
    }
    
    #pragma warning disable CS8618
    
    public partial class TokenResponse {

        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string TokenType { get; set; }
        public int ExpiresIn { get; set; }
        public object Scope { get; set; }
        
    }

}