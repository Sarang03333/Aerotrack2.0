using AeroTrack.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using AeroTrack.Api.Auth;
using AeroTrack.Api.Services;
using AeroTrack.Api.Infrastructure.Middleware;

var builder = WebApplication.CreateBuilder(args);
 
// DbContext
var conn = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(conn));
 
// Controllers (MVC)
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // This prevents the infinite loop error you see in the console
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });
 
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
builder.Services.AddSwaggerGen(c =>
{
    // Optional: explicit doc info
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "AeroTrack.Api", Version = "v1" });
 
    // ===== Swagger security (Bearer JWT) =====
    var bearerScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter JWT as: **Bearer {token}**",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",            // must be exactly "bearer"
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };
 
    c.AddSecurityDefinition("Bearer", bearerScheme);
 
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        [bearerScheme] = Array.Empty<string>()
    });
});
 
// ===== AuthN / AuthZ =====
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var key = builder.Configuration["Jwt:Key"]!;
        options.TokenValidationParameters = new TokenValidationParameters
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
    options.AddPolicy("AnyRole", p => p.RequireRole("Admin", "Maintenance", "InventoryManager", "ComplianceOfficer"));
});
//This handles the registration automatically for all your controllers
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
// We use AddScoped because DbContext is Scoped (created per request)
builder.Services.AddScoped<IAircraftService, AircraftService>();
builder.Services.AddScoped<IUserService, DbUserService>();
builder.Services.AddScoped<IMaintenanceService, MaintenanceService>();
builder.Services.AddScoped<IComplianceService, ComplianceService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IReportService, ReportService>();

 
var app = builder.Build();
 
Console.WriteLine($"ENV: {app.Environment.EnvironmentName}");
 
app.UseCors("SpaDev");
app.UseMiddleware<ExceptionMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(o =>
{
    o.SwaggerEndpoint("/swagger/v1/swagger.json", "AeroTrack.Api v1");
    o.DisplayRequestDuration();
});
 
app.UseAuthentication();   
app.UseAuthorization();
 
app.MapControllers();
 
app.MapGet("/", () => Results.Ok(new { service = "AeroTrack API", ok = true }));
 
app.Run();