using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using GoogleFormsClone.Models;
using GoogleFormsClone.DTOs.Auth;
using BCrypt.Net;
using GoogleFormsClone.DTOs.User;
using MongoDB.Bson.Serialization.Attributes;

namespace GoogleFormsClone.Services;

public class AuthService
{
    private readonly IMongoCollection<User> _users;
    private readonly IMongoCollection<RefreshToken> _refreshTokens;
    private readonly JwtSettings _jwtSettings;

    public AuthService(IMongoClient client, IOptions<MongoDbSettings> dbSettings, IOptions<JwtSettings> jwtOptions)
    {
        var database = client.GetDatabase(dbSettings.Value.DatabaseName);
        _users = database.GetCollection<User>("Users");
        _refreshTokens = database.GetCollection<RefreshToken>("RefreshTokens");
        _jwtSettings = jwtOptions.Value;
    }

    // --- REGISTER ---
    public async Task<User> RegisterUserAsync(User user)
    {
        var existing = await _users.Find(u => u.Email == user.Email).FirstOrDefaultAsync();
        if (existing != null)
            throw new InvalidOperationException("Email already registered.");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;

        await _users.InsertOneAsync(user);
        return user;
    }

    // --- LOGIN / AUTHENTICATE ---
    public async Task<AuthResponse?> AuthenticateUserAsync(string email, string password)
    {
        var user = await _users.Find(u => u.Email == email && u.IsActive).FirstOrDefaultAsync();
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return null;

        // Create access token
        var accessToken = GenerateJwtToken(user);

        // Create refresh token
        var refreshToken = new RefreshToken
        {
            Token = Guid.NewGuid().ToString(),
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
            IsActive = true
        };
        await _refreshTokens.InsertOneAsync(refreshToken);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                Role = user.Role,
                AvatarUrl = user.AvatarUrl
            }
        };
    }

    // --- REFRESH TOKEN ---
    public async Task<AuthResponse?> RefreshTokenAsync(string token)
    {
        var existingToken = await _refreshTokens.Find(rt => rt.Token == token && rt.IsActive)
                                                .FirstOrDefaultAsync();
        if (existingToken == null || existingToken.ExpiresAt < DateTime.UtcNow)
            return null;

        var user = await _users.Find(u => u.Id == existingToken.UserId).FirstOrDefaultAsync();
        if (user == null)
            return null;

        // Deactivate old token
        existingToken.IsActive = false;
        await _refreshTokens.ReplaceOneAsync(rt => rt.Id == existingToken.Id, existingToken);

        // Generate new tokens
        var accessToken = GenerateJwtToken(user);
        var refreshToken = new RefreshToken
        {
            Token = Guid.NewGuid().ToString(),
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
            IsActive = true
        };
        await _refreshTokens.InsertOneAsync(refreshToken);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                Role = user.Role,
                AvatarUrl = user.AvatarUrl
            }
        };
    }

    // --- REVOKE REFRESH TOKEN ---
    public async Task<bool> RevokeRefreshTokenAsync(string token)
    {
        var existingToken = await _refreshTokens.Find(rt => rt.Token == token && rt.IsActive)
                                                .FirstOrDefaultAsync();
        if (existingToken == null)
            return false;

        existingToken.IsActive = false;
        var result = await _refreshTokens.ReplaceOneAsync(rt => rt.Id == existingToken.Id, existingToken);
        return result.ModifiedCount > 0;
    }

    // --- HELPER: Generate JWT ---
    private string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return tokenHandler.WriteToken(token);
    }
}

// --- Refresh Token Model ---
public class RefreshToken
{
    [BsonId]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string UserId { get; set; } = null!;
    
    public string Token { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public bool IsActive { get; set; } = true;
}
