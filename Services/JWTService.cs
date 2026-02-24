using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HrApi.Models;
using HrBackend.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NuGet.Packaging;

public class JWTService
{
    private readonly JWT _jwt;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager; 
    public JWTService(IOptions<JWT> jwt, UserManager<IdentityUser> userManager , RoleManager<IdentityRole> roleManager)
    {
        _jwt = jwt.Value;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<string> GenerateToken(IdentityUser user)
    {
       var userClaims = await _userManager.GetClaimsAsync(user);
        var userRoles = await _userManager.GetRolesAsync(user);

       // var roleClaims = userRoles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();

        var claims = new List<Claim>
        {
             new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
        };//.Union(userClaims).Union(roleClaims);
        claims.AddRange(userClaims);
        foreach (var role in userRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
            var roleEntity = await _roleManager.FindByNameAsync(role);
            var roleClaims = await _roleManager.GetClaimsAsync(roleEntity);
            claims.AddRange(roleClaims);
        }
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.key));
        Console.WriteLine("GEN KEY: " + _jwt.key);
        var creds = new SigningCredentials(key,SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddDays(double.Parse(_jwt.DurationInDays));

        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires:expires,
            signingCredentials:creds
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}