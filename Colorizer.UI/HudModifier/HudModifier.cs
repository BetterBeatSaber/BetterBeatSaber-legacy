using UnityEngine;

namespace BetterBeatSaber.Colorizer.UI.HudModifier; 

public abstract class HudModifier : MonoBehaviour {

    public partial class BaseOptions {

        public bool Enabled { get; set; } = true;
        public bool Glow { get; set; } = true;

    }

}