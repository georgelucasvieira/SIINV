using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthService.Api.Data;
using AuthService.Api.Features.GetCurrentUser;
using AuthService.Api.Features.Login;
using AuthService.Api.Features.RefreshToken;
using AuthService.Api.Features.Register;
using AuthService.Api.Shared;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Prometheus;
using Serilog;

var builder = WebApplication.CreateBuilder(args);


Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();


builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AuthConnection")));


var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName);
builder.Services.Configure<JwtSettings>(jwtSettings);

var secret = jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret não configurado");
var key = Encoding.UTF8.GetBytes(secret);


JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        NameClaimType = ClaimTypes.NameIdentifier,
        RoleClaimType = ClaimTypes.Role
    };
});

builder.Services.AddAuthorization();


builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));


builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);


builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Auth Service API",
        Version = "v1",
        Description = "Microsserviço de Autenticação e Autorização"
    });


    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();


await DbInitializer.InitializeAsync(app.Services);


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Auth Service API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseSerilogRequestLogging();

app.UseAuthentication();
app.UseAuthorization();


app.MapLoginEndpoint();
app.MapRegisterEndpoint();
app.MapRefreshTokenEndpoint();
app.MapGetCurrentUserEndpoint();


app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }))
    .WithName("HealthCheck")
    .WithTags("Health")
    .AllowAnonymous();

app.UseHttpMetrics();
app.MapMetrics();

app.Run();


public partial class Program { }
