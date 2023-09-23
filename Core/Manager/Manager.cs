using BetterBeatSaber.Core.Interfaces;
using BetterBeatSaber.Core.Utilities;

using IPA.Logging;

namespace BetterBeatSaber.Core.Manager; 

public abstract class Manager<T> : ConstructableSingleton<T>, IInitializable, IEnableable where T : Manager<T> {

    private Logger? _logger;
    protected Logger Logger => _logger ??= BetterBeatSaber.Logger.GetChildLogger(GetType().Name);

    #region Init & Exit

    public virtual void Init() { }
    public virtual void Exit() { }

    #endregion

    #region Enable & Disable

    public virtual void Enable() { }
    public virtual void Disable() { }

    #endregion
    
}