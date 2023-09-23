namespace BetterBeatSaber.Server.Jobs; 

public abstract class Job<T> : BackgroundService where T : Job<T> {

    protected abstract TimeSpan Delay { get; }
    protected abstract TimeSpan Interval { get; }
    
    protected IServiceScopeFactory ScopeFactory { get; }
    protected ILogger<T> Logger { get; }

    protected Job(IServiceScopeFactory scopeFactory,ILogger<T> logger) {
        ScopeFactory = scopeFactory;
        Logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        
        Logger.LogInformation("Started Job: \"{Name}\"", GetName());
        
        Thread.Sleep(Delay);
        
        while (!stoppingToken.IsCancellationRequested) {
            await Task.Run(Run, stoppingToken);
            Thread.Sleep(Interval);
        }
        
        Logger.LogInformation("Stopped Job: \"{Name}\"", GetName());
        
    }

    protected abstract Task Run();

    public string GetName() {
        var name = typeof(T).Name;
        return name.EndsWith("Job") ? name[..^3] : name;
    }

}