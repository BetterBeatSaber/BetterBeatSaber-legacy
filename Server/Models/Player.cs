using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;

using BetterBeatSaber.Server.Interfaces;
using BetterBeatSaber.Shared.Enums;
using BetterBeatSaber.Shared.Models;

using Newtonsoft.Json;

namespace BetterBeatSaber.Server.Models; 

#pragma warning disable CS8618
#pragma warning disable CA1067

// ReSharper disable InconsistentNaming

public sealed class Player : ISharedConvertable<Shared.Models.Player> {

    private static readonly Encoding DefaultEncryptionEncoding = Encoding.UTF8;
    
    private const int EncryptionKeyLength = 32;
    private const int EncryptionIvLength = 16;
    
    [Key] // Steam ID
    public ulong Id { get; init; }
    
    public string Name { get; set; }
    
    public string AvatarUrl { get; set; }
    
    public PlayerRole Role { get; set; }

    public PlayerFlag Flags { get; set; }
    
    #region Leaderboard

    #region Score Saber

    public double? ScoreSaberPp { get; set; }
    public uint? ScoreSaberGlobalRank { get; set; }
    public uint? ScoreSaberCountryRank { get; set; }
    public string? ScoreSaberCountry { get; set; }

    #endregion
    
    #region Beat Leader

    public double? BeatLeaderPp { get; set; }
    public uint? BeatLeaderGlobalRank { get; set; }
    public uint? BeatLeaderCountryRank { get; set; }
    public string? BeatLeaderCountry { get; set; }

    #endregion

    #endregion
    
    public DateTime LastUpdate { get; set; }
    
    #region Encryption

    [NotMapped]
    [JsonIgnore]
    public byte[] EncryptionKey => SHA512.HashData(Encoding.UTF8.GetBytes(Id.ToString())).Take(EncryptionKeyLength).ToArray();

    [NotMapped]
    [JsonIgnore]
    public byte[] EncryptionIv => SHA512.HashData(Encoding.UTF8.GetBytes(Id.ToString())).Take(EncryptionIvLength).ToArray();

    #endregion
    
    #region Cipher

    public byte[] Encrypt(byte[] data) {

        return data;
        
        var aes = Aes.Create();
        
        aes.Mode = CipherMode.CBC;
        aes.KeySize = EncryptionKeyLength * 8;
        aes.Padding = PaddingMode.Zeros;
        aes.Key = EncryptionKey;
        aes.IV = EncryptionIv;

        return PerformCryptography(data, aes.CreateEncryptor());
        
    }

    public byte[] Encrypt(string data) {
        return DefaultEncryptionEncoding.GetBytes(data);
        //return Encrypt(DefaultEncryptionEncoding.GetBytes(data));
    }
    
    public byte[] Decrypt(byte[] data) {

        return data;
        
        var aes = Aes.Create();
        
        aes.Mode = CipherMode.CBC;
        aes.KeySize = EncryptionKeyLength * 8;
        aes.Padding = PaddingMode.Zeros;
        aes.Key = EncryptionKey;
        aes.IV = EncryptionIv;

        return PerformCryptography(data, aes.CreateDecryptor());
        
    }

    public string DecryptToString(byte[] data) {
        return DefaultEncryptionEncoding.GetString(data);
        //return DefaultEncryptionEncoding.GetString(Decrypt(data));
    }
    
    private static byte[] PerformCryptography(byte[] data, ICryptoTransform cryptoTransform) {
        
        using var memoryStream = new MemoryStream();
        using var cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write);
        
        cryptoStream.Write(data, 0, data.Length);
        cryptoStream.FlushFinalBlock();

        return memoryStream.ToArray();

    }

    #endregion

    public Shared.Models.Player ToSharedModel() => new() {
        Id = Id,
        Name = Name,
        AvatarUrl = AvatarUrl,
        Role = Role,
        Flags = Flags,
        ScoreSaber = Flags.HasFlag(PlayerFlag.HasScoreSaber) ? new Leaderboard {
            Country = ScoreSaberCountry!,
            Pp = ScoreSaberPp!.Value,
            GlobalRank = ScoreSaberGlobalRank!.Value,
            LocalRank = ScoreSaberCountryRank!.Value
        } : null,
        BeatLeader = Flags.HasFlag(PlayerFlag.HasBeatLeader) ? new Leaderboard {
            Country = BeatLeaderCountry!,
            Pp = BeatLeaderPp!.Value,
            GlobalRank = BeatLeaderGlobalRank!.Value,
            LocalRank = BeatLeaderCountryRank!.Value
        } : null
    };

}