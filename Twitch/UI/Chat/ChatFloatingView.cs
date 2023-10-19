using System.Collections.Generic;

using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;

using BetterBeatSaber.Core.UI;
using BetterBeatSaber.Twitch.Shared.Models;

using JetBrains.Annotations;

using UnityEngine;

namespace BetterBeatSaber.Twitch.UI.Chat; 

public sealed class ChatFloatingView : FloatingView<ChatFloatingView> {

    public override Vector2 ScreenSize => new(120f, 140f);

    [UsedImplicitly]
    [UIComponent(nameof(MessageList))]
    public readonly CustomListTableData MessageList = null!;
    
    private MessageListTableData? _messageListTableData;
    
    [UIAction(PostParseEvent)]
    private void PostParse() {

        _messageListTableData = new MessageListTableData(MessageList);
        
        //Twitch.OnChatMessageReceived += OnChatMessageReceived;
        
    }

    #region Event Handlers

    private void OnChatMessageReceived(ChatMessage _) => _messageListTableData?.Reload();

    #endregion
    
    private class MessageListTableData : CachedListData<ChatMessage, ChatFloatingViewCell> {

        public MessageListTableData(CustomListTableData table, bool reloadData = false) : base(table, reloadData) { }

        public override List<ChatMessage> Items => Twitch.Instance.RecentMessages;

    }
    
}