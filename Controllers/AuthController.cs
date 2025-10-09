using Microsoft.AspNetCore.Mvc;
using GoogleFormsClone.Models;
using GoogleFormsClone.Services;
using GoogleFormsClone.DTOs;
using GoogleFormsClone.DTOs.Auth;

namespace GoogleFormsClone.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("Email and password are required.");
        if (request.Password != request.PasswordConfirm)
            return BadRequest("Passwords do not match.");

        try
        {
            var user = new User
            {
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Name = string.IsNullOrWhiteSpace(request.Name) ? request.Email.Split('@')[0] : request.Name,
                AvatarUrl = "default-avatar.png",
                Role = "user",
                IsActive = true,
                Preferences = new UserPreferences(),
                Stats = new UserStats(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createdUser = await _authService.RegisterUserAsync(user);
            return Ok(new { createdUser.Id, createdUser.Email });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto request)
    {
        var response = await _authService.RefreshTokenAsync(request.RefreshToken);
        if (response == null)
            return Unauthorized();

        return Ok(response);
    }
    
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshRequestDto request)
    {
        await _authService.RevokeRefreshTokenAsync(request.RefreshToken);
        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthRequest request)
    {
        var response = await _authService.AuthenticateUserAsync(request.Email, request.Password);
        if (response == null)
            return Unauthorized(new { error = "Invalid credentials." });

        return Ok(response);
    }
}