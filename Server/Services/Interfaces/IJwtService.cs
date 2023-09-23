using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BetterBeatSaber.Server.Services.Interfaces; 

public interface IJwtService {

    public string Sign(ClaimsIdentity claims, DateTime? expires = null);
    
    public JwtSecurityToken? Validate(string token);

}