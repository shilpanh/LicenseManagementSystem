using LicenseService.API.Infrastructure;
using LicenseService.API.Application;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Hangfire;
using Hangfire.MemoryStorage;
using LicenseService.API.Data;
using LicenseService.API.Infrastructure.Tenant;
using Microsoft.EntityFrameworkCore;
using LicenseService.API.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using LicenseService.API.Application.Interfaces;
using LicenseService.API.Infrastructure.Identity;

var builder = WebApplication.CreateBuilder(args);

// Configuration
var configuration = builder.Configuration;

// Register DbContext with SQLite
builder.Services.AddDbContext<LicenseDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "LicenseService.API",
        Version = "v1"
    });

    // Add JWT Authentication to Swagger
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token.\n\nExample: Bearer eyJhbGciOiJIUzI1NiIs..."
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
            new string[] {}
        }
    });
});


// EF Core + SQL Server
builder.Services.AddInfrastructure(configuration);

// MediatR (CQRS)
builder.Services.AddMediatR(typeof(LicenseService.API.Application.AssemblyMarker));

// Tenant resolution middleware/service
builder.Services.AddScoped<ITenantProvider, HeaderTenantProvider>();

// Authentication - JWT (for demo we accept HS256 with signing key from appsettings)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]))
        };
    });

// Authorization
builder.Services.AddAuthorization();

// Hangfire (in-memory for local demo)
builder.Services.AddHangfire(config => config.UseMemoryStorage());
builder.Services.AddHangfireServer();

// Application services
builder.Services.AddApplicationServices();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContext, UserContext>();


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();
app.UseAuthentication();
// Middleware
app.UseMiddleware<TenantMiddleware>(); // sets Tenant in scoped ITenantProvider

app.UseAuthorization();

app.UseHangfireDashboard("/hangfire", new DashboardOptions { Authorization = new[] { new HangfireAllowAllFilter() } });
app.MapControllers();

app.Run();
