using System;

using BetterBeatSaber.Core.Interfaces;

namespace BetterBeatSaber.Core.Manager.Service; 

public abstract class Service : IInitializable, IEnableable {

    #region Init & Exit

    public virtual void Init() { }
    public virtual void Exit() { }

    #endregion

    #region Enable & Disable

    public abstract void Enable();
    public abstract void Disable();

    #endregion

}

public abstract class Service<T> : Service where T : Service<T> {

    public static T Instance { get; private set; } = null!;

    protected Service() {
        if (Instance != null)
            throw new Exception("Only one instance can exist at a time!");
        Instance = (T) this;
    }
    
}