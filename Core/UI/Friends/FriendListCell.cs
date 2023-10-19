using BeatSaberMarkupLanguage.Attributes;
using BetterBeatSaber.Core.Extensions;
using BetterBeatSaber.Core.TextMeshPro;
using BetterBeatSaber.Core.UI.Components;
using BetterBeatSaber.Shared.Enums;
using BetterBeatSaber.Shared.Models;

using JetBrains.Annotations;

using TMPro;

namespace BetterBeatSaber.Core.UI.Friends; 

public sealed class FriendListCell : ListCell<Player> {

    [UsedImplicitly]
    [UIComponent(nameof(Avatar))]
    internal readonly AvatarComponent Avatar = null!;
    
    [UsedImplicitly]
    [UIComponent(nameof(Name))]
    internal readonly TMP_Text Name = null!;
    
    [UsedImplicitly]
    [UIComponent(nameof(Status))]
    internal readonly TMP_Text Status = null!;

    public override void Populate(Player player) {
        
        Name.text = player.Name;
        
        if(player.Flags.HasFlag(PlayerFlag.HasCustomName))
            TextMeshProAddon.Apply(Name);
        
        Avatar.SetPlayer(player);
        
        UpdateStatus(player.GetPresence(), player.GetLobby());
        
    }

    public void UpdateStatus(IPresence? presence, Shared.Models.Lobby? lobby) =>
        Status.text = presence.GetStatusText(lobby);

}