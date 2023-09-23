using Discord.WebSocket;

namespace BetterBeatSaber.Server.Discord.Interfaces; 

public interface IDiscordService {

    public DiscordSocketClient Client { get; }

}