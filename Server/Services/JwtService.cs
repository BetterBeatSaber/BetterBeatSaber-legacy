using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using BetterBeatSaber.Server.Services.Interfaces;

using Microsoft.IdentityModel.Tokens;

namespace BetterBeatSaber.Server.Services; 

public sealed class JwtService : IJwtService {

    private readonly string _issuer;
    private readonly SymmetricSecurityKey _key;

    public JwtService(IConfiguration configuration) {
        _issuer = configuration["Jwt:Issuer"]!;
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
    }

    public string Sign(ClaimsIdentity claims, DateTime? expires = null) {
        
        var tokenHandler = new JwtSecurityTokenHandler();
        
        return tokenHandler.WriteToken(tokenHandler.CreateToken(new SecurityTokenDescriptor {
            Subject = new ClaimsIdentity(claims),
            Expires = expires,
            Issuer = _issuer,
            SigningCredentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature)
        }));
        
    }

    public JwtSecurityToken? Validate(string token) {
        try {
            
            new JwtSecurityTokenHandler().ValidateToken(token, new TokenValidationParameters {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _key,
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out var validatedToken);
            
            return (JwtSecurityToken) validatedToken;
            
        } catch (Exception) {
            return null;
        }
    }

}