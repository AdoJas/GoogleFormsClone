using Microsoft.AspNetCore.Mvc;
using GoogleFormsClone.Models;
using GoogleFormsClone.Services;

namespace GoogleFormsClone.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    // GET: api/User/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest(new { error = "User ID is required." });

        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound(new { error = "User not found." });

        return Ok(user);
    }

    // POST: api/User
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] User user)
    {
        if (user == null)
            return BadRequest(new { error = "User data is required." });

        // If you want to validate fields manually:
        if (string.IsNullOrWhiteSpace(user.Email) || string.IsNullOrWhiteSpace(user.PasswordHash))
            return BadRequest(new { error = "Email and Password are required." });

        var createdUser = await _userService.CreateUserAsync(user);
        return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, createdUser);
    }

    // PUT: api/User/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] User user)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest(new { error = "User ID is required." });

        var updatedUser = await _userService.UpdateUserAsync(id, user);
        if (updatedUser == null)
            return NotFound(new { error = "User not found." });

        return Ok(updatedUser);
    }

    // DELETE: api/User/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest(new { error = "User ID is required." });

        var result = await _userService.DeleteUserAsync(id);
        if (!result)
            return NotFound(new { error = "User not found." });

        return NoContent();
    }

    // GET: api/User
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }
}
