using Microsoft.AspNetCore.Mvc;
using GoogleFormsClone.Models;
using GoogleFormsClone.Services;
using GoogleFormsClone.DTOs.Auth;
using Microsoft.AspNetCore.Authorization;

namespace GoogleFormsClone.Controllers;

[Authorize]
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly IWebHostEnvironment _env;

    public AuthController(AuthService authService, IWebHostEnvironment env)
    {
        _authService = authService;
        _env = env;
    }

    [AllowAnonymous]
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
                PasswordHash = request.Password,
                Name = string.IsNullOrWhiteSpace(request.Name)
                    ? request.Email.Split('@')[0]
                    : request.Name,
                AvatarUrl = null,
                Role = "user",
                IsActive = true,
                Preferences = new UserPreferences(),
                Stats = new UserStats(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var authResponse = await _authService.RegisterAndLoginUserAsync(user);
            SetAuthCookies(authResponse);
            return Ok(new { user = authResponse.User });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthRequest request)
    {
        var response = await _authService.AuthenticateUserAsync(request.Email, request.Password);
        if (response == null)
            return Unauthorized(new { error = "Invalid credentials." });

        SetAuthCookies(response);
        return Ok(new { user = response.User });
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        if (!Request.Cookies.TryGetValue("refreshToken", out var token))
            return Unauthorized();

        var response = await _authService.RefreshTokenAsync(token);
        if (response == null)
            return Unauthorized();

        SetAuthCookies(response);
        return Ok(new { user = response.User });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        if (Request.Cookies.TryGetValue("refreshToken", out var token))
            await _authService.RevokeRefreshTokenAsync(token);

        ClearAuthCookies();
        return Ok();
    }

    private void SetAuthCookies(AuthResponse response)
	{
    	bool isDev = _env.IsDevelopment();

    	var accessOptions = new CookieOptions
    	{
        	HttpOnly = true,
        	Secure = true, 
        	SameSite = SameSiteMode.None, 
        	Expires = DateTime.UtcNow.AddMinutes(30),
        	Path = "/"
    	};

    	var refreshOptions = new CookieOptions
    	{
        	HttpOnly = true,
        	Secure = true,
        	SameSite = SameSiteMode.None,
        	Expires = DateTime.UtcNow.AddDays(7),
        	Path = "/"
    	};

    	Response.Cookies.Append("accessToken", response.AccessToken, accessOptions);
    	Response.Cookies.Append("refreshToken", response.RefreshToken, refreshOptions);
	}

    private void ClearAuthCookies()
	{
    	var options = new CookieOptions
    	{
        	Path = "/",
        	Secure = true,
        	SameSite = SameSiteMode.None
    	};

    	Response.Cookies.Delete("accessToken", options);
    	Response.Cookies.Delete("refreshToken", options);
	}
}
