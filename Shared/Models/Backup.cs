using System;

using BetterBeatSaber.Shared.Serialization;

namespace BetterBeatSaber.Shared.Models; 

public struct Backup : ISerializable {

    public string GameVersion { get; set; }
    public BackupData Data { get; set; }
    
    public string[]? Songs { get; set; } // Song Hashes
    public byte[]? SettingsFile { get; set; }
    public byte[]? PlayerDataFile { get; set; }
    public byte[]? UserDataArchive { get; set; }
    public byte[]? LibrariesArchive { get; set; }
    public byte[]? PluginsArchive { get; set; }
    public byte[]? CustomSabersArchive { get; set; }
    public byte[]? CustomNotesArchive { get; set; }

    public void Serialize(ByteBuffer buffer) {
        
        buffer.WriteString(GameVersion);

        var data = BackupData.None;
        if (Songs != null)
            data |= BackupData.Songs;
        if (SettingsFile != null)
            data |= BackupData.SettingsFile;
        if (PlayerDataFile != null)
            data |= BackupData.PlayerDataFile;
        if (UserDataArchive != null)
            data |= BackupData.UserDataArchive;
        if (LibrariesArchive != null)
            data |= BackupData.LibrariesArchive;
        if (PluginsArchive != null)
            data |= BackupData.PluginsArchive;
        if (CustomSabersArchive != null)
            data |= BackupData.CustomSabersArchive;
        if (CustomNotesArchive != null)
            data |= BackupData.CustomNotesArchive;
        
        buffer.WriteEnum(data);
        
        if(Songs != null)
            buffer.WriteStringArray(Songs);
        if(SettingsFile != null)
            buffer.WriteByteArray(SettingsFile);
        if(PlayerDataFile != null)
            buffer.WriteByteArray(PlayerDataFile);
        if(UserDataArchive != null)
            buffer.WriteByteArray(UserDataArchive);
        if(LibrariesArchive != null)
            buffer.WriteByteArray(LibrariesArchive);
        if(PluginsArchive != null)
            buffer.WriteByteArray(PluginsArchive);
        if(CustomSabersArchive != null)
            buffer.WriteByteArray(CustomSabersArchive);
        if(CustomNotesArchive != null)
            buffer.WriteByteArray(CustomNotesArchive);
        
    }

    public void Deserialize(ByteBuffer buffer) {
        
        GameVersion = buffer.ReadString();
        Data = buffer.ReadEnum<BackupData>();
        
        if(Data.HasFlag(BackupData.Songs))
            Songs = buffer.ReadStringArray();
        if(Data.HasFlag(BackupData.SettingsFile))
            SettingsFile = buffer.ReadByteArray();
        if(Data.HasFlag(BackupData.PlayerDataFile))
            PlayerDataFile = buffer.ReadByteArray();
        if(Data.HasFlag(BackupData.UserDataArchive))
            UserDataArchive = buffer.ReadByteArray();
        if(Data.HasFlag(BackupData.LibrariesArchive))
            LibrariesArchive = buffer.ReadByteArray();
        if(Data.HasFlag(BackupData.PluginsArchive))
            PluginsArchive = buffer.ReadByteArray();
        if(Data.HasFlag(BackupData.CustomSabersArchive))
            CustomSabersArchive = buffer.ReadByteArray();
        if(Data.HasFlag(BackupData.CustomNotesArchive))
            CustomNotesArchive = buffer.ReadByteArray();
        
    }

    [Flags]
    public enum BackupData : ushort {

        None = 0,
        Songs,
        SettingsFile,
        PlayerDataFile,
        UserDataArchive,
        LibrariesArchive,
        PluginsArchive,
        CustomSabersArchive,
        CustomNotesArchive

    }

}