using BetterBeatSaber.Server.Discord.Interfaces;
using BetterBeatSaber.Server.Services;

using Discord;
using Discord.WebSocket;

namespace BetterBeatSaber.Server.Discord; 

public sealed class DiscordService : LifetimeService<DiscordService>, IDiscordService {

    public DiscordSocketClient Client { get; }

    private readonly string _token;

    public DiscordService(ILogger<DiscordService> logger, IHostApplicationLifetime applicationLifetime, IConfiguration configuration) : base(logger, applicationLifetime) {
        
        Client = new DiscordSocketClient();
        
        Client.Ready += OnReady;
        
        _token = configuration.GetValue<string>("Discord:BotToken")!;
        
    }

    public override async Task Init() {
        await Client.LoginAsync(TokenType.Bot, _token);
        await Client.StartAsync();
    }

    protected override async void Exit() {
        await Client.StopAsync();
    }

    private Task OnReady() {
        return Task.CompletedTask;
    }
    
}