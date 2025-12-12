using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _config;
    public AuthController(UserManager<ApplicationUser> userManager, IConfiguration config)
    {
        _userManager = userManager;
        _config = config;
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email
        };
        var result = await _userManager.CreateAsync(user, dto.password);
        if (!result.succeeded)
        {
            return BadRequest(result.errors);
        }
        return Ok("User Registered successfully");
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, dto.password))
        {
            return Unauthorized();
        }

        //Generate JWT
        var claims = new List<System.Security.Claims.Claim>
        {
            // new(JwtRegisteredClaimNames.Sub,user.email.ToString()),
            //new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };
    }
}
