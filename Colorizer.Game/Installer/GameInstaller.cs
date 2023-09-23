using BetterBeatSaber.Colorizer.Game.Colorizer;
using BetterBeatSaber.Colorizer.Game.Config;

namespace BetterBeatSaber.Colorizer.Game.Installer; 

public sealed class GameInstaller : Core.Zenject.Installer {

    public override void Install() {
        InstallConditional(() => Container.InstantiateComponentOnNewGameObject<DustColorizer>(nameof(DustColorizer)), GameColorizerConfig.Instance.ColorizeDust);
        InstallConditional(() => Container.InstantiateComponentOnNewGameObject<FeetColorizer>(nameof(FeetColorizer)), GameColorizerConfig.Instance.ColorizeFeet);
        InstallConditional(() => Container.InstantiateComponentOnNewGameObject<PlayersPlaceColorizer>(nameof(PlayersPlaceColorizer)), GameColorizerConfig.Instance.ColorizePlayersPlace);
    }

    public override void Uninstall() {
    }

}