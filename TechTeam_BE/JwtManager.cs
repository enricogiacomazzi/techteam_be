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
    private SymmetricSecurityKey secretKey => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

    public TokenValidationParameters GetTokenValidationParameters()
    {
        return new TokenValidationParameters
        {
            ValidIssuer = tenant,
            ValidAudience = tenant,
            IssuerSigningKey = secretKey,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true
        };
    }

    public TokenResponse SignIn(string username)
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();
        var uuid = Guid.NewGuid();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("Id", uuid.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Email, username),
            }),
            Expires = DateTime.UtcNow.AddMinutes(5),
            Audience = tenant,
            Issuer = tenant,
            SigningCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha512Signature)
        };
        
        var tokenDescriptorRefresh = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("Id", uuid.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Email, username),
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            Audience = tenant,
            Issuer = tenant,
            SigningCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha512Signature)
        };

        var accessToken = jwtTokenHandler.CreateToken(tokenDescriptor);
        var refreshToken = jwtTokenHandler.CreateToken(tokenDescriptorRefresh);

        return new TokenResponse(jwtTokenHandler.WriteToken(accessToken), jwtTokenHandler.WriteToken(refreshToken));
    }

    public TokenResponse Refresh(string refreshToken)
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();
        var tkn = jwtTokenHandler.ReadToken(refreshToken) as JwtSecurityToken;
        var user = tkn.Claims.SingleOrDefault(x => x.Type == "sub")?.Value;
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