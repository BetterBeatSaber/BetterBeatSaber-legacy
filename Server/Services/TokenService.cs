using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using BetterBeatSaber.Server.Models;
using BetterBeatSaber.Server.Services.Enums;
using BetterBeatSaber.Server.Services.Interfaces;

namespace BetterBeatSaber.Server.Services; 

public sealed class TokenService : ITokenService {

    public const string JwtIdClaimName = "_id";
    public const string JwtRoleClaimName = ClaimTypes.Role;
    public const string JwtTypeClaimName = "_type";
    public const string JwtCustomDataClaimName = "_data";

    private readonly IJwtService _jwtService;
    private readonly IPlayerService _playerService;

    public TokenService(IJwtService jwtService, IPlayerService playerService) {
        _jwtService = jwtService;
        _playerService = playerService;
    }

    public string CreateToken(Player player, TokenType type, string? customData = null, TimeSpan? expiration = null) {

        var expires = DateTime.Now.Add(expiration ?? TimeSpan.FromDays(7));
        
        var claims = new List<Claim>() {
            new Claim(JwtIdClaimName, player.Id.ToString()),
            new Claim(JwtRoleClaimName, player.Role.ToString()),
            new Claim(JwtTypeClaimName, type.ToString())
        };
        
        if(customData != null)
            claims.Add(new Claim(JwtCustomDataClaimName, customData));
        
        return _jwtService.Sign(new ClaimsIdentity(claims), expires);
        
    }

    public bool VerifyToken(string token, TokenType neededTokenType = TokenType.All) =>
        ValidateToken(token, neededTokenType) != null;

    public string? VerifyTokenWithCustomData(string token, TokenType neededTokenType = TokenType.All) =>
        ValidateToken(token, neededTokenType)?.Claims.FirstOrDefault(claim => claim.Type == JwtCustomDataClaimName)?.Value;

    public async Task<Player?> GetPlayerFromToken(string token, TokenType neededTokenType = TokenType.All) {
        
        var data = ValidateToken(token, neededTokenType);
        if (data == null)
            return null;

        if (!ulong.TryParse(data.Claims.First(claim => claim.Type == JwtIdClaimName).Value, out var id))
            return null;
        
        return await _playerService.GetById(id);
        
    }
    
    private JwtSecurityToken? ValidateToken(string token, TokenType neededTokenType = TokenType.All) {
        
        var jwtSecurityToken = _jwtService.Validate(token);
        
        if (!ulong.TryParse(jwtSecurityToken?.Claims.FirstOrDefault(claim => claim.Type == JwtIdClaimName)?.Value, out var _))
            return null;
        
        var type = jwtSecurityToken?.Claims.FirstOrDefault(claim => claim.Type == JwtTypeClaimName)?.Value;
        if (type == null || !Enum.TryParse(typeof(TokenType), type, out var tokenType))
            return null;

        if (!neededTokenType.HasFlag((TokenType) tokenType))
            return null;

        return jwtSecurityToken;

    }

}