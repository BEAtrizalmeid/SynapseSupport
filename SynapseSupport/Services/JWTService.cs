using Microsoft.IdentityModel.Tokens;
using SynapseSupport.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SynapseSupport.Services;

public class JwtService
{
    private readonly string _secret;
    private readonly string _expMinutes;
    private readonly string _issuer;
    private readonly string _audience;

    public JwtService(IConfiguration config)
    {
        _secret = config["Jwt:Secret"] ?? throw new ArgumentNullException("Jwt:Secret missing");
        _expMinutes = config["Jwt:ExpiresMinutes"] ?? "60";
        _issuer = config["Jwt:Issuer"] ?? "SynapseSupport";
        _audience = config["Jwt:Audience"] ?? "SynapseSupportClient";
    }

    public string GenerateToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Perfil.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(double.Parse(_expMinutes)),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}