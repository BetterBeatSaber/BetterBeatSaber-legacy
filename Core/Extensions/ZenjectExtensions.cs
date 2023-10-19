using Zenject;

namespace BetterBeatSaber.Core.Extensions; 

public static class ZenjectExtensions {

    public static DiContainer? GetContainer(this MonoInstallerBase installer) =>
        installer.GetProperty<MonoInstallerBase, DiContainer>("Container");

}