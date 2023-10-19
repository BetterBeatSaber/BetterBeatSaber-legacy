using System.Xml;

using BetterBeatSaber.Core.Manager;
using BetterBeatSaber.Core.Utilities;
using BetterBeatSaber.Shared.Models;

using UnityEngine;

namespace BetterBeatSaber.Core.UI.SDK.Components; 

public class Avatar : Image {

    public ulong? PlayerId { get; set; }
    
    public override GameObject Create(Transform parent, XmlNode node) {
        
        var gameObject = base.Create(parent, node);
        gameObject.name = nameof(Avatar);
        
        if (ImageView == null)
            return gameObject;
        
        ImageView.preserveAspect = true;
        
        if(PlayerId != null)
            SetPlayerId(PlayerId.Value);
        
        return gameObject;
        
    }

    public void SetPlayer(Player player) =>
        SetPlayerId(player.Id);
    
    public void SetPlayerId(ulong playerId) =>
        ThreadDispatcher.Enqueue(PlayerManager.Instance.DownloadAvatarAndApply(playerId, ImageView));

}