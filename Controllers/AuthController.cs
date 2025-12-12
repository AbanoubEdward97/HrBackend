using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using HrApi.Models;
using HrBackend.Dtos.Auth;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    private readonly JWTService _jWTService;
    public AuthController(UserManager<ApplicationUser> userManager, IConfiguration config, JWTService jWTService)
    {
        _userManager = userManager;
        _jWTService = jWTService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
        {
            return Unauthorized("Invalid Email or Password");

        }
        var validPassword = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!validPassword)
        {
            return Unauthorized("Invalid Email or Password");
        }

        //create the token
        var token = await _jWTService.GenerateToken(user);
        return Ok(new LoginResponseDto
        {
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddDays(double.Parse("7"))
        });
    }
}

