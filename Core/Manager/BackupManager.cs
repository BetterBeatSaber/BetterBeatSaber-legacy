using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

using BetterBeatSaber.Core.Extensions;
using BetterBeatSaber.Core.Game;
using BetterBeatSaber.Core.Utilities;
using BetterBeatSaber.Shared.Serialization;

namespace BetterBeatSaber.Core.Manager; 

public sealed class BackupManager : Manager<BackupManager> {

    private static readonly List<string> IgnoredLibraries = new() {
        "0Harmony.dll",
        "0Harmony.xml",
        "Hive.Versioning.dll",
        "Hive.Versioning.xml",
        "Ionic.Zip.dll",
        "Mono.Cecil.dll",
        "Mono.Cecil.Mdb.dll",
        "Mono.Cecil.Pdb.dll",
        "Mono.Cecil.Rocks.dll",
        "MonoMod.RuntimeDetour.dll",
        "MonoMod.RuntimeDetour.xml",
        "MonoMod.Utils.dll",
        "MonoMod.Utils.xml",
        "Newtonsoft.Json.dll",
        "Newtonsoft.Json.xml",
        "SemVer.dll",
        "SemVer.pdb"
    };
    
    #region Init & Exit

    public override void Init() {
        ThreadDispatcher.EnqueueOffMain(() => {
            File.WriteAllBytes(@"C:\Users\steve\Desktop\bk.bin", CreateBackupByteArray());
        });
    }

    public override void Exit() {
    }
    
    #endregion

    #region Create
    
    public Shared.Models.Backup CreateBackup(
        bool backupSettings = true,
        bool backupSongs = true,
        bool backupUserData = true,
        bool backupLibs = true,
        bool backupPlugins = true,
        bool backupCustomSabers = false,
        bool backupCustomNotes = false
        ) {

        var backup = new Shared.Models.Backup {
            GameVersion = BeatSaber.Version.ToString()
        };

        if (backupSettings) {
            backup.SettingsFile = File.ReadAllBytes(Path.Combine(BeatSaber.DataDirectory, "settings.cfg"));
            backup.PlayerDataFile = File.ReadAllBytes(Path.Combine(BeatSaber.DataDirectory, "PlayerData.dat"));
        }
        
        if (backupSongs)
            backup.Songs = MapManager.Instance.Songs.Select(song => song.GetHash()).ToArray();

        if (backupUserData)
            backup.UserDataArchive = CreateArchive(BeatSaber.UserDataDirectory);

        if (backupLibs)
            backup.LibrariesArchive = CreateArchive(BeatSaber.LibrariesDirectory, IgnoredLibraries);

        if (backupPlugins)
            backup.PluginsArchive = CreateArchive(BeatSaber.PluginsDirectory);

        if (backupCustomSabers && Directory.Exists(BeatSaber.CustomSabersDirectory))
            backup.CustomSabersArchive = CreateArchive(BeatSaber.CustomSabersDirectory);
        
        if (backupCustomNotes && Directory.Exists(BeatSaber.CustomNotesDirectory))
            backup.CustomNotesArchive = CreateArchive(BeatSaber.CustomNotesDirectory);
        
        return backup;
        
    }

    public byte[] CreateBackupByteArray() {
        var backup = CreateBackup();
        var buffer = new ByteBuffer(ByteBuffer.MaxSize);
        buffer.Write(backup);
        using var memoryStream = new MemoryStream();
        using var deflateStream = new DeflateStream(memoryStream, CompressionLevel.Optimal);
        deflateStream.Write(buffer.ToArray(), 0, buffer.Position);
        return memoryStream.ToArray();
    }
    
    #endregion

    #region Restore

    public void RestoreBackup(Shared.Models.Backup backup) {
        
    }

    #endregion
    
    #region Utilitis

    private static byte[] CreateArchive(string path) =>
        CreateArchive(path, Enumerable.Empty<string>());
    
    private static byte[] CreateArchive(string path, IEnumerable<string> ignoredFiles) {
        using var memoryStream = new MemoryStream();
        using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create);
        AddFolderToArchive(path, archive, path, ignoredFiles);
        return memoryStream.ToArray();
    }

    private static void AddFolderToArchive(string sourceFolder, ZipArchive archive, string rootFolder, IEnumerable<string> ignoredFiles) {
        
        foreach (var filePath in Directory.GetFiles(sourceFolder)) {
            
            // ReSharper disable once PossibleMultipleEnumeration
            if(ignoredFiles.Contains(Path.GetFileName(filePath)))
                continue;
            
            var relativePath = GetRelativePath(rootFolder, filePath);
            
            var entry = archive.CreateEntry(relativePath);
            
            using var entryStream = entry.Open();
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            fileStream.CopyTo(entryStream);
            
        }
        
        foreach (var subfolder in Directory.GetDirectories(sourceFolder))
            // ReSharper disable once PossibleMultipleEnumeration
            AddFolderToArchive(subfolder, archive, rootFolder, ignoredFiles);
        
    }
    
    private static string GetRelativePath(string absolutePath, string basePath) =>
        Uri.UnescapeDataString(new Uri(basePath).MakeRelativeUri(new Uri(absolutePath)).ToString().Replace('/', Path.DirectorySeparatorChar));

    #endregion

}