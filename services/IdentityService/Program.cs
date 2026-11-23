using BookNook.IdentityService.Configurations;
using BookNook.IdentityService.Data;
using BookNook.IdentityService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Register application services.
builder.Services.AddScoped<PasswordService>();
builder.Services.AddScoped<RegistrationService>();
builder.Services.AddScoped<LoginService>();
builder.Services.AddScoped<JwtService>();

// Configure JWT settings.
var jwtSettings = new JwtSettings
{
    SecretKey = builder.Configuration["JwtSettings:SecretKey"]
        ?? throw new InvalidOperationException("JWT secret key is missing."),

    Issuer = builder.Configuration["JwtSettings:Issuer"]
        ?? throw new InvalidOperationException("JWT issuer is missing."),

    Audience = builder.Configuration["JwtSettings:Audience"]
        ?? throw new InvalidOperationException("JWT audience is missing."),

    ExpirationMinutes = int.Parse(
        builder.Configuration["JwtSettings:ExpirationMinutes"]
        ?? "60")
};

builder.Services.AddSingleton(jwtSettings);


// Configure JWT authentication.
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,

            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,

            ValidateLifetime = true, // Rejects expired tokens

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.SecretKey)
            ),

            ClockSkew = TimeSpan.Zero // Removes the default expiration grace period, a token becomes invalid as soon as it reaches its expiration time.
        };
    });


// Configure PostgreSQL database connection.
var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Database connection string is missing.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Authentication must come before authorization.
app.UseAuthentication(); // identifies who is making the request.
app.UseAuthorization(); // determines whether that user is allowed to access the endpoint.

app.MapControllers();

app.Run();

