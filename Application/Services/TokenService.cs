using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services;

public class TokenService(UserManager<User> userManager) : ITokenService
{
    public async Task<string> CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName!)
        };

        var roles = await userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var envKey = AuthOptionsHelper.GetSecretKey();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(envKey));
        var exp = AuthOptionsHelper.GetTokenExpirationTime();

        var jwt = new JwtSecurityToken(
            AuthOptionsHelper.GetIssuer(),
            AuthOptionsHelper.GetAudience(),
            claims,
            expires: DateTime.UtcNow.AddSeconds(exp),
            notBefore: DateTime.UtcNow,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}