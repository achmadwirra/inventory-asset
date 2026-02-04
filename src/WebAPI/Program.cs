using System.Text;
using FluentValidation;
using Inventory.Application.DTOs;
using Inventory.Application.Interfaces;
using Inventory.Application.UseCases;
using Inventory.Application.Validators;
using Inventory.Infrastructure.Auth;
using Inventory.Infrastructure.AuditLogging;
using Inventory.Infrastructure.Persistence;
using Inventory.Infrastructure.Repositories;
using Inventory.Infrastructure.Seeding;
using Inventory.WebAPI.Middleware;
using Inventory.WebAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped<IAssetRepository, AssetRepository>();
builder.Services.AddScoped<IAssetAssignmentRepository, AssetAssignmentRepository>();
builder.Services.AddScoped<IAssetCategoryRepository, AssetCategoryRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();

// Services
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Use Cases
builder.Services.AddScoped<LoginUseCase>();
builder.Services.AddScoped<CreateAssetUseCase>();
builder.Services.AddScoped<GetAssetsUseCase>();
builder.Services.AddScoped<AssignAssetUseCase>();
builder.Services.AddScoped<ReturnAssetUseCase>();
builder.Services.AddScoped<GetCategoriesUseCase>();
builder.Services.AddScoped<CreateCategoryUseCase>();
builder.Services.AddScoped<UpdateCategoryUseCase>();
builder.Services.AddScoped<DeleteCategoryUseCase>();
builder.Services.AddScoped<GetUsersByRoleUseCase>();

// Validators
builder.Services.AddScoped<IValidator<CreateAssetRequest>, CreateAssetRequestValidator>();
builder.Services.AddScoped<IValidator<AssignAssetRequest>, AssignAssetRequestValidator>();
builder.Services.AddScoped<IValidator<CreateCategoryRequest>, CreateCategoryRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateCategoryRequest>, UpdateCategoryRequestValidator>();

// JWT Authentication
var jwtKey = builder.Configuration["JwtSettings:SecretKey"] ?? "super_secret_key_12345678901234567890";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"] ?? "InventoryApp",
            ValidAudience = builder.Configuration["JwtSettings:Audience"] ?? "InventoryClient",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Seed Database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await context.Database.MigrateAsync();
    await DatabaseSeeder.SeedAsync(context);
}

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
