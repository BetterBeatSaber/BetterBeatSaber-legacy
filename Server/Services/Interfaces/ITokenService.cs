using BetterBeatSaber.Server.Models;
using BetterBeatSaber.Server.Services.Enums;

namespace BetterBeatSaber.Server.Services.Interfaces; 

public interface ITokenService {

    public string CreateToken(Player player, TokenType type, string? customData = null, TimeSpan? expiration = null);
    
    public bool VerifyToken(string token, TokenType neededTokenType = TokenType.All);

    public string? VerifyTokenWithCustomData(string token, TokenType neededTokenType = TokenType.All);

    public Task<Player?> GetPlayerFromToken(string token, TokenType neededTokenType = TokenType.All);

}