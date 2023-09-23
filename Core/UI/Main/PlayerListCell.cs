using BeatSaberMarkupLanguage.Attributes;

using BetterBeatSaber.Core.TextMeshPro;
using BetterBeatSaber.Core.UI.Components;
using BetterBeatSaber.Shared.Enums;
using BetterBeatSaber.Shared.Models;

using JetBrains.Annotations;

using TMPro;

namespace BetterBeatSaber.Core.UI.Main; 

public sealed class PlayerListCell : ListCell<Player> {

    [UsedImplicitly]
    [UIComponent(nameof(Avatar))]
    public readonly AvatarComponent Avatar = null!;
    
    [UsedImplicitly]
    [UIComponent(nameof(Name))]
    public readonly TMP_Text Name = null!;

    public override void Populate(Player player) {
        
        Name.text = player.Name;
        
        Avatar.SetPlayer(player);
        
        if(player.Flags.HasFlag(PlayerFlag.HasCustomName))
            TextMeshProAddon.Apply(Name);
        
    }
    
}