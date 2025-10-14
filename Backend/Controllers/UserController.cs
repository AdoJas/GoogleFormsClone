using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GoogleFormsClone.Models;
using GoogleFormsClone.Services;
using GoogleFormsClone.DTOs.User;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace GoogleFormsClone.Controllers;

[ApiController]
[Route("api/users")]
[Authorize] 
public class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var user = await _userService.GetUserByIdAsync(userId);
        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpPut("me")]
    [Authorize]
    public async Task<IActionResult> UpdateCurrentUser([FromBody] UserUpdateDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var updatedUser = await _userService.UpdateUserAsync(userId, dto);
        if (updatedUser == null)
            return NotFound();

        return Ok(updatedUser);
    }
    
    [HttpDelete("me")]
    [Authorize]
    public async Task<IActionResult> DeleteCurrentUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) 
                     ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var result = await _userService.DeleteUserAndDataAsync(userId);

        if (!result)
            return NotFound(new { error = "User not found or could not be deleted." });

        return NoContent();
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> GetUser(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest(new { error = "User ID is required." });

        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound(new { error = "User not found." });

        return Ok(user);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] UserUpdateDto request)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest(new { error = "User ID is required." });

        var updatedUser = await _userService.UpdateUserAsync(id, request);
        if (updatedUser == null)
            return NotFound(new { error = "User not found." });

        return Ok(updatedUser);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteUserAndData(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest(new { error = "User ID is required." });

        var result = await _userService.DeleteUserAndDataAsync(id);
        if (!result)
            return NotFound(new { error = "User not found." });

        return NoContent();
    }
    [HttpGet]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }
}
