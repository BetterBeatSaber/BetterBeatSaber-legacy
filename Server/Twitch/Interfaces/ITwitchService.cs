using TwitchLib.Api.Interfaces;
using TwitchLib.Client.Interfaces;
using TwitchLib.PubSub.Interfaces;

namespace BetterBeatSaber.Server.Twitch.Interfaces; 

public interface ITwitchService {

    public ITwitchClient TwitchClient { get; }
    public ITwitchPubSub TwitchPubSub { get; }
    public ITwitchAPI TwitchAPI { get; }
    
}