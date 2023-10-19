using System.Collections.Generic;
using System.Linq;

using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components.Settings;

using BetterBeatSaber.Core.UI;
using BetterBeatSaber.Twitch.Config;
using BetterBeatSaber.Twitch.Shared.Models;

using JetBrains.Annotations;

using TMPro;

namespace BetterBeatSaber.Twitch.UI.Config; 

public sealed class UserView : View<UserView> {

    public User User { get; set; }
    public TwitchUserConfig Config => TwitchConfig.Instance.GetUserConfig(User.Id);
    public bool Updated { get; private set; }

    #region UI Components

    [UsedImplicitly]
    [UIComponent(nameof(Name))]
    public readonly TMP_Text Name = null!;
    
    [UsedImplicitly]
    [UIComponent(nameof(Voice))]
    public readonly DropDownListSetting Voice = null!;

    #endregion
    
    #region UI Values

    public string CustomName {
        get => Config.CustomName ?? User.Name;
        set {
            
            if (value == Config.CustomName)
                return;
            
            Config.CustomName = value;
            
            Updated = true;
            
        }
    }

    public string CustomLanguage {
        get => Config.CustomLanguage ?? TwitchTTSConfig.DefaultLanguage;
        set {
            
            if (value == Config.CustomLanguage)
                return;
            
            Config.CustomLanguage = value;
            
            Updated = true;
            
        }
    }

    public string CustomVoice {
        get => Config.CustomVoice ?? TwitchTTSConfig.DefaultVoice;
        set {
            
            if (value == Config.CustomVoice)
                return;
            
            Config.CustomVoice = value;
            
            Updated = true;
            
        }
    }

    public float CustomRate {
        get => Config.CustomRate ?? TwitchTTSConfig.DefaultRate;
        set {
            
            var val = (int) value;
            
            if (val == Config.CustomRate)
                return;
            
            Config.CustomRate = val;
            
            Updated = true;
            
        }
    }
    
    public float CustomPitch {
        get => Config.CustomPitch ?? TwitchTTSConfig.DefaultPitch;
        set {

            var val = (int) value;
            
            if (val == Config.CustomPitch)
                return;
            
            Config.CustomPitch = val;
            
            Updated = true;
            
        }
    }
    
    public float CustomVolume {
        get => Config.CustomVolume ?? TwitchTTSConfig.DefaultVolume;
        set {
            
            var val = (int) value;
            
            if (val == Config.CustomVolume)
                return;
            
            Config.CustomVolume = val;
            
            Updated = true;
            
        }
    }
    
    [UsedImplicitly]
    [UIValue(nameof(Languages))]
    private List<object> Languages => Twitch.Instance.Voices.Keys.Cast<object>().ToList();

    [UsedImplicitly]
    [UIValue(nameof(Voices))]
    private List<object> Voices => new() {
        TwitchTTSConfig.DefaultVoice
    };
    
    #endregion

    #region UI Actions
    
    [UIAction(nameof(OnChangeLanguage))]
    public void OnChangeLanguage(string language) {
        Voice.values = Twitch.Instance.Voices[language].Cast<object>().ToList();
        Voice.Value = Voice.values.First();
    }

    #endregion
    
}