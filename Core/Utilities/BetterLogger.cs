using System;

using IPA.Logging;

using LiteNetLib;

namespace BetterBeatSaber.Core.Utilities; 

public sealed class BetterLogger : INetLogger {

    public Logger Logger { get; }

    public BetterLogger(Logger logger) {
        Logger = logger;
    }

    public BetterLogger GetChildLogger(string name) => new(Logger.GetChildLogger(name));
    
    public void WriteNet(NetLogLevel level, string str, params object[] args) =>
        Logger.Log(ConvertLevel(level), string.Format(str, args));

    public void Info(string message, params object[] args) => Info(string.Format(message, args));
    public void Info(string message) => Logger.Info(message);
    
    public void Warn(string message, params object[] args) => Warn(string.Format(message, args));
    public void Warn(string message) => Logger.Warn(message);
    public void Warn(Exception exception) => Logger.Warn(exception);

    public void Error(string message, params object[] args) => Error(string.Format(message, args));
    public void Error(string message) => Logger.Error(message);
    public void Error(Exception exception) => Logger.Error(exception);
    
    public void Debug(string message, params object[] args) => Debug(string.Format(message, args));
    public void Debug(string message) => Logger.Debug(message);
    public void Debug(Exception exception) => Logger.Debug(exception);

    public void Log(Logger.Level level, string message) => Logger.Log(level, message);
    
    private static Logger.Level ConvertLevel(NetLogLevel logLevel) {
        return logLevel switch {
            NetLogLevel.Trace => Logger.Level.Trace,
            NetLogLevel.Info => Logger.Level.Info,
            NetLogLevel.Warning => Logger.Level.Warning,
            NetLogLevel.Error => Logger.Level.Error,
            _ => Logger.Level.None
        };
    }

}