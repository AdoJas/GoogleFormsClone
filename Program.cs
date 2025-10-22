using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using GoogleFormsClone.Models;
using GoogleFormsClone.Services;
using FluentValidation;
using FluentValidation.AspNetCore;


var builder = WebApplication.CreateBuilder(args);

//
// ──────────────────────────────────────────────────────────────
//  1. CONFIGURATION
// ──────────────────────────────────────────────────────────────
//
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));

builder.Services.AddSingleton<RedisService>();


//
// ──────────────────────────────────────────────────────────────
//  2. MONGODB CLIENT SETUP
// ──────────────────────────────────────────────────────────────
//
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});

builder.Services.AddSingleton<MongoDbService>();

//
// ──────────────────────────────────────────────────────────────
//  3. JWT AUTHENTICATION SETUP
// ──────────────────────────────────────────────────────────────
//
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
if (jwtSettings == null || string.IsNullOrEmpty(jwtSettings.SecretKey))
    throw new InvalidOperationException("JWT secret key is missing in configuration.");

var key = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Services.AddAuthorization();

//
// ──────────────────────────────────────────────────────────────
//  4. SERVICE REGISTRATION
// ──────────────────────────────────────────────────────────────
//
builder.Services.AddSingleton<AuthService>();
builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<FormService>();
builder.Services.AddSingleton<ResponseService>();
builder.Services.AddSingleton<IFormService, FormService>();


//
// ──────────────────────────────────────────────────────────────
//  5. CONTROLLERS + SWAGGER
// ──────────────────────────────────────────────────────────────
//
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Add JWT auth button in Swagger
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer"
    });
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

//
// ──────────────────────────────────────────────────────────────
//  6. BUILD & MIDDLEWARE PIPELINE
// ──────────────────────────────────────────────────────────────
//
var app = builder.Build();

// Swagger for development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// HTTPS redirection (optional)
app.UseHttpsRedirection();

// Enable JWT auth
app.UseAuthentication();
app.UseAuthorization();

// Map controllers
app.MapControllers();

app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();
