namespace BetterBeatSaber.Core.Backup; 

public sealed class BackupConfig {

    public bool CreateBackupOnStartup { get; set; } = true;
    
    public bool BackupCustomLevels { get; set; } = true;
    
    public bool BackupUserData { get; set; } = true;
    
    // ReSharper disable once InconsistentNaming
    public bool BackupWIPs { get; set; } = false;
    
}