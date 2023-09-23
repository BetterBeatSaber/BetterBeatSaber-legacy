using BetterBeatSaber.Server.Interfaces;

namespace BetterBeatSaber.Server.Services; 

public abstract class LifetimeService<T> : IInitializable where T : LifetimeService<T> {

    protected ILogger<T> Logger { get; }
    
    protected LifetimeService(ILogger<T> logger, IHostApplicationLifetime applicationLifetime) {
        Logger = logger;
        applicationLifetime.ApplicationStopping.Register(Exit);
    }

    public abstract Task Init();
    protected abstract void Exit();

}