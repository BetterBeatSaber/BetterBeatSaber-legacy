using System.Diagnostics;

using BeatSaberMarkupLanguage.Tags;

using BetterBeatSaber.Core.Manager;
using BetterBeatSaber.Core.Utilities;
using BetterBeatSaber.Shared.Models;

using HMUI;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BetterBeatSaber.Core.UI.Components; 

public sealed class AvatarComponent : ImageView, IPointerClickHandler {

    public Player Player { get; private set; }

    public void SetPlayer(Player player) {
        Player = player;
        ThreadDispatcher.Enqueue(PlayerManager.Instance.DownloadAvatarAndApply(player, this));
    }
    
    public void OnPointerClick(PointerEventData _) =>
        Process.Start($"https://steamcommunity.com/profiles/{Player.Id}");
    
    // TODO: Add hover-hint
    public sealed class Tag : BSMLTag {

        public override string[] Aliases => new[] { "avatar" };
        
        public override GameObject CreateObject(Transform parent) {

            var gameObject = new GameObject("Avatar") {
                layer = 5
            };
            
            var image = gameObject.AddComponent<AvatarComponent>();
            
            image.material = BeatSaberMarkupLanguage.Utilities.ImageResources.NoGlowMat;
            image.sprite = BeatSaberMarkupLanguage.Utilities.ImageResources.BlankSprite;
            image.rectTransform.SetParent(parent, false);
            image.rectTransform.sizeDelta = new Vector2(20f, 20f);
            image.preserveAspect = true;

            gameObject.AddComponent<LayoutElement>();

            return gameObject;

        }

    }

}