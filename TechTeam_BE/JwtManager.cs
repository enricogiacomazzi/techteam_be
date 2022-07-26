using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace TechTeam_BE;

internal record TokenResponse(string accessToken, string refreshToken);

internal class JwtManager
{
    private const string tenant = "techteam";
    private const string secret = "ijurkbdlhmklqacwqzdxmkkhvqowlyqa";
    private const string refreshSecret = "ijurkbdlhmklpoiuyzdxmkkhvqowlyqa";
    private SymmetricSecurityKey secretKey => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
    private SymmetricSecurityKey refreshSecretKey => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(refreshSecret));

    public TokenValidationParameters GetTokenValidationParameters() => GetTokenValidationParameters(secretKey);

    private TokenValidationParameters GetTokenValidationParameters(SymmetricSecurityKey key)
    {
        return new TokenValidationParameters {
            ValidIssuer = tenant,
            ValidAudience = tenant,
            IssuerSigningKey = key,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero
        };
    } 
        

    public TokenResponse SignIn(string username)
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();
        var uuid = Guid.NewGuid();
        var accessToken = jwtTokenHandler.CreateToken(GetSecurityTokenDescriptor(uuid, username, DateTime.UtcNow.AddMinutes(5), secretKey));
        var refreshToken = jwtTokenHandler.CreateToken(GetSecurityTokenDescriptor(uuid, username, DateTime.UtcNow.AddHours(1), refreshSecretKey));
        return new TokenResponse(jwtTokenHandler.WriteToken(accessToken), jwtTokenHandler.WriteToken(refreshToken));
    }

    private SecurityTokenDescriptor GetSecurityTokenDescriptor(Guid id, string username, DateTime expiring, SymmetricSecurityKey key)
    {
        return new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("Id", id.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Email, username),
            }),
            Expires = expiring,
            Audience = tenant,
            Issuer = tenant,
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature)
        };
    }

    public TokenResponse Refresh(string refreshToken)
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();
        SecurityToken validated = null;
        var aaa = jwtTokenHandler.ValidateToken(refreshToken, GetTokenValidationParameters(refreshSecretKey), out validated);
        var tkn = validated as JwtSecurityToken;
        var user = tkn?.Claims.SingleOrDefault(x => x.Type == "sub")?.Value;
        if (user != null)
        {
            return SignIn(user);
        }
        else
        {
            throw new Exception("errore refresh");
        }
    } 
}