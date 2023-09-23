using BetterBeatSaber.Colorizer.Game.Colorizer;
using BetterBeatSaber.Colorizer.Game.Config;

namespace BetterBeatSaber.Colorizer.Game.Installer; 

public sealed class MenuInstaller : Core.Zenject.Installer {

    public override void Install() {
        InstallConditional(() => Container.InstantiateComponentOnNewGameObject<DustColorizer>(nameof(DustColorizer)), GameColorizerConfig.Instance.ColorizeDust);
        InstallConditional(() => Container.InstantiateComponentOnNewGameObject<FeetColorizer>(nameof(FeetColorizer)), GameColorizerConfig.Instance.ColorizeFeet);
    }

    public override void Uninstall() {
    }

}