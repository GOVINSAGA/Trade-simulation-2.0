using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TradeSimulation.API.Models;
using TradeSimulation.API.Services;

namespace TradeSimulation.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly TokenService _tokenService;
    private readonly WalletService _walletService;

    public AuthController(UserManager<AppUser> userManager, TokenService tokenService, WalletService walletService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _walletService = walletService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FullName))
            return BadRequest(new { message = "Full name is required" });
        if (string.IsNullOrWhiteSpace(request.Email))
            return BadRequest(new { message = "Email is required" });
        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
            return BadRequest(new { message = "Password must be at least 6 characters" });

        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
            return BadRequest(new { message = "An account with this email already exists" });

        var user = new AppUser
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return BadRequest(new { message = errors });
        }

        await _walletService.InitializeWalletForUserAsync(user.Id);

        var token = _tokenService.CreateToken(user);
        return Ok(new AuthResponse(token, user.FullName, user.Email));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return Unauthorized(new { message = "Invalid email or password" });

        var validPassword = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!validPassword)
            return Unauthorized(new { message = "Invalid email or password" });

        var token = _tokenService.CreateToken(user);
        return Ok(new AuthResponse(token, user.FullName, user.Email!));
    }

    [HttpGet("me")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return Unauthorized();

        return Ok(new { FullName = user.FullName, Email = user.Email });
    }
}
