using BeatSaberMarkupLanguage.Attributes;

using BetterBeatSaber.Core.Manager;
using BetterBeatSaber.Core.TextMeshPro;
using BetterBeatSaber.Core.UI.Components;
using BetterBeatSaber.Shared.Enums;
using BetterBeatSaber.Shared.Models;

using JetBrains.Annotations;

using TMPro;

using UnityEngine.UI;

namespace BetterBeatSaber.Core.UI.Lobby; 

public sealed class LobbyPlayerListCell : ListCell<Player> {

    #region UI Components

    [UsedImplicitly]
    [UIComponent(nameof(Avatar))]
    public readonly AvatarComponent Avatar = null!;
    
    [UsedImplicitly]
    [UIComponent(nameof(Name))]
    public readonly TMP_Text Name = null!;
    
    [UsedImplicitly]
    [UIComponent(nameof(Status))]
    public readonly TMP_Text Status = null!;

    [UsedImplicitly]
    [UIComponent(nameof(KickButton))]
    public readonly Button KickButton = null!;

    [UsedImplicitly]
    [UIComponent(nameof(PromoteButton))]
    public readonly Button PromoteButton = null!;
    
    #endregion

    public Player? Player { get; private set; }
    
    public bool IsSelf { get; private set; }
    public bool AmIOwner { get; private set; }
    
    public override void Populate(Player player) {

        Player = player;
        IsSelf = Player.Value.Equals(AuthManager.Instance.CurrentPlayer);
        AmIOwner = AuthManager.Instance.CurrentPlayer.Equals(LobbyManager.Instance.Lobby?.Owner);
        
        Name.text = player.Name;
        
        if(player.Flags.HasFlag(PlayerFlag.HasCustomName))
            TextMeshProAddon.Apply(Name);
        
        Avatar.SetPlayer(player);

        UpdateButtons();

    }

    [UIAction(nameof(KickPlayer))]
    public void KickPlayer() {
        if (Player != null)
            LobbyManager.Instance.KickPlayer(Player);
    }

    [UIAction(nameof(PromotePlayer))]
    public void PromotePlayer() {
        if (Player != null)
            LobbyManager.Instance.PromotePlayer(Player);
    }

    private void UpdateButtons() {
        SetActiveIf(KickButton.gameObject, AmIOwner && !IsSelf);
        SetActiveIf(PromoteButton.gameObject, AmIOwner && !IsSelf);
    }
    
}