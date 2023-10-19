using BetterBeatSaber.Core.Interfaces;
using BetterBeatSaber.Core.Utilities;

namespace BetterBeatSaber.Core.Manager; 

public abstract class Manager<T> : ConstructableSingleton<T>, IInitializable, IEnableable where T : Manager<T> {

    private BetterLogger? _logger;
    protected BetterLogger Logger => _logger ??= BetterBeatSaber.Logger.GetChildLogger(GetType().Name);

    #region Init & Exit

    public virtual void Init() { }
    public virtual void Exit() { }

    #endregion

    #region Enable & Disable

    public virtual void Enable() { }
    public virtual void Disable() { }

    #endregion
    
}