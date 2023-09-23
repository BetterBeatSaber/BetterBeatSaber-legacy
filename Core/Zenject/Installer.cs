using System;

using JetBrains.Annotations;

using Zenject;

namespace BetterBeatSaber.Core.Zenject; 

public abstract class Installer : IDisposable {

    [UsedImplicitly]
    public readonly DiContainer Container = null!;

    public abstract void Install();
    public virtual void Uninstall() {}

    public void Dispose() {
        GC.SuppressFinalize(this);
    }

    protected static void InstallConditional(Action action, bool condition) {
        if (condition)
            action();
    }
    
}