using System;

using BetterBeatSaber.Core.UI;
using BetterBeatSaber.Twitch.Shared.Models;

namespace BetterBeatSaber.Twitch.UI.Chat; 

public sealed class ChatFloatingViewCell : ListCell<ChatMessage> {

    public override void Populate(ChatMessage data) {
        Console.WriteLine(data.User.Name + " - " + data.Message);
    }

}