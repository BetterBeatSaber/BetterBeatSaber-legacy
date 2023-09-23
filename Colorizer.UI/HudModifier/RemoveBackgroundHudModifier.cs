using UnityEngine;

namespace BetterBeatSaber.Colorizer.UI.HudModifier; 

public sealed class RemoveBackgroundHudModifier : HudModifier {

    public void Start() {
        
        var leftPanelBackground = GameObject.Find("LeftPanel/BG");
        if (leftPanelBackground != null) {
            DestroyImmediate(leftPanelBackground);
        }
        
        var rightPanelBackground = GameObject.Find("RightPanel/BG");
        if (rightPanelBackground != null) {
            DestroyImmediate(rightPanelBackground);
        }
        
    }

}