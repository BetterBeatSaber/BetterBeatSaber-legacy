using System;
using System.Collections.Generic;
using System.Linq;

using BetterBeatSaber.Core.Manager.Audio;

using UnityEngine;

namespace BetterBeatSaber.Core.Manager; 

public sealed class AssetManager : Manager<AssetManager> {

    private AssetBundle? _assetBundle;

    internal static Dictionary<string, Texture2D> CountryFlags = new();

    public override void Init() {
        
        _assetBundle = AssetBundle.LoadFromStream(typeof(AssetManager).Assembly.GetManifestResourceStream("BetterBeatSaber.Core.Resources.assets"));
        if (_assetBundle == null)
            return;
            
        LoadAudio();
        LoadCountryFlags();
        
        _assetBundle.Unload(false);
        
    }

    public override void Exit() { }

    private void LoadAudio() {
        foreach (Manager.Audio.Audio audio in Enum.GetValues(typeof(Manager.Audio.Audio))) {
            var audioClip = _assetBundle!.LoadAsset<AudioClip>(audio.ToString().ToLower());
            if(audioClip != null)
                AudioManager.AudioClips[audio] = audioClip;
            else
                Console.WriteLine("Failed to load AudioClip for {0}", audio.ToString());
        }
    }

    private void LoadCountryFlags() {
        
        if (_assetBundle == null)
            return;

        foreach (var path in _assetBundle.GetAllAssetNames().Where(assetName => assetName.Contains("/flags/"))) {

            var country = path.Substring(path.Length - 6, 2);
            if(CountryFlags.ContainsKey(country))
                continue;
            
            CountryFlags.Add(country, _assetBundle.LoadAsset<Texture2D>(country));
            
        }
        
    }

}