using AeroTrack.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AeroTrack.Api.Auth;

var builder = WebApplication.CreateBuilder(args);

// DbContext
var conn = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(conn));

// Controllers (MVC)
builder.Services.AddControllers();

// CORS for Angular dev
builder.Services.AddCors(options =>
{
    options.AddPolicy("SpaDev", policy =>
        policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ===== AuthN / AuthZ =====
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var key = builder.Configuration["Jwt:Key"]!;
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", p => p.RequireRole("Admin"));
    options.AddPolicy("MaintenanceWrite", p => p.RequireRole("Admin", "Maintenance"));
    options.AddPolicy("MaintenanceTransition", p => p.RequireRole("Admin", "Maintenance"));
    options.AddPolicy("InventoryWrite", p => p.RequireRole("Admin", "InventoryManager"));
    options.AddPolicy("ComplianceWrite", p => p.RequireRole("Admin", "ComplianceOfficer"));
    options.AddPolicy("AnyRole", p => p.RequireRole("Admin","Maintenance","InventoryManager","ComplianceOfficer"));
});

// In-memory users (swap for DB later)
builder.Services.AddSingleton<IUserService, InMemoryUserService>();

var app = builder.Build();

Console.WriteLine($"ENV: {app.Environment.EnvironmentName}");

app.UseCors("SpaDev");

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => Results.Ok(new { service = "AeroTrack API", ok = true }));

app.Run();