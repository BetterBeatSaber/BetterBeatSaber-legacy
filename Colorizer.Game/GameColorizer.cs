using System.Collections;
using System.Reflection;

using BetterBeatSaber.Colorizer.Game.Config;
using BetterBeatSaber.Colorizer.Game.Installer;
using BetterBeatSaber.Colorizer.Game.UI;
using BetterBeatSaber.Colorizer.Game.Utilities;
using BetterBeatSaber.Core.Utilities;
using BetterBeatSaber.Core.Zenject;

using UnityEngine;

using Module = BetterBeatSaber.Core.Module;

namespace BetterBeatSaber.Colorizer.Game;

// ReSharper disable once UnusedType.Global
public sealed class GameColorizer : Module {
    
    public override void Init() {
        CreateConfig<GameColorizerConfig>();
    }

    public override void Enable() {
        
        RegisterView<ConfigView>();
        
        AddInstaller<GameInstaller>(Location.StandardPlayer);
        AddInstaller<MenuInstaller>(Location.Menu);
        
        Patch();
        
        ThreadDispatcher.Enqueue(LoadAssets());
        
    }

    public override void Disable() {
        
        RemoveInstaller<MenuInstaller>();
        RemoveInstaller<GameInstaller>();
        
        UnregisterView();
        
        Unpatch();

    }

    private static IEnumerator LoadAssets() {
        
        var bundleRequest = AssetBundle.LoadFromStreamAsync(Assembly.GetExecutingAssembly().GetManifestResourceStream("BetterBeatSaber.Colorizer.Game.Resources.resources"));
        yield return bundleRequest;
        
        var assetBundle = bundleRequest.assetBundle;
        if (assetBundle == null)
            yield break;

        var fillMatRequest = assetBundle.LoadAssetAsync<Material>("OutlineFill");
        yield return fillMatRequest;
        Outline.OutlineFillMaterialSource = (fillMatRequest.asset as Material)!;
        
        var maskMatRequest = assetBundle.LoadAssetAsync<Material>("OutlineMask");
        yield return maskMatRequest;
        Outline.OutlineMaskMaterialSource = (maskMatRequest.asset as Material)!;
        
        assetBundle.Unload(false);

    }
    
}