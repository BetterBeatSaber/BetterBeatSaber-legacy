using BeatSaberMarkupLanguage.Attributes;

using BetterBeatSaber.Core.Manager;
using BetterBeatSaber.Core.UI.Components;
using BetterBeatSaber.Core.Utilities;
using BetterBeatSaber.Shared.Models;

using JetBrains.Annotations;

using TMPro;

using UnityEngine.UI;

namespace BetterBeatSaber.Core.UI.Main; 

// ReSharper disable UnusedMember.Local

public sealed class FriendRequestCell : ListCell<Player> {

    [UsedImplicitly]
    [UIComponent(nameof(Avatar))]
    public readonly AvatarComponent Avatar = null!;
    
    [UsedImplicitly]
    [UIComponent(nameof(Name))]
    public readonly TMP_Text Name = null!;

    [UsedImplicitly]
    [UIComponent(nameof(AcceptButton))]
    public readonly Button AcceptButton = null!;

    [UsedImplicitly]
    [UIComponent(nameof(DeclineButton))]
    public readonly Button DeclineButton = null!;

    private Player _player;
    
    public override void Populate(Player player) {
        _player = player;
        Name.text = player.Name;
        Avatar.SetPlayer(player);
    }

    private void SetButtons(bool state) {
        AcceptButton.interactable = state;
        DeclineButton.interactable = state;
    }

    [UIAction(nameof(AcceptRequest))]
    private void AcceptRequest() {
        SetButtons(false);
        FriendManager.Instance.AcceptRequest(_player).ContinueWith(task => {
            if (task.IsCompleted && task.Result) {
                // ReSharper disable once Unity.NoNullPropagation
                MainView.Instance?.FriendRequestTable?.Reload();
            } else {
                ThreadDispatcher.Enqueue(() => SetButtons(true));
            }
        });
    }
    
    [UIAction(nameof(DeclineRequest))]
    private void DeclineRequest() {
        SetButtons(false);
        FriendManager.Instance.DeclineRequest(_player).ContinueWith(task => {
            if (task.IsCompleted && task.Result) {
                // ReSharper disable once Unity.NoNullPropagation
                MainView.Instance?.FriendRequestTable?.Reload();
            } else {
                ThreadDispatcher.Enqueue(() => SetButtons(true));
            }
        });
    }

}