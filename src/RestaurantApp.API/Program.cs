using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RestaurantApp.API.Hubs;
using RestaurantApp.Infrastructure;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Serilog;

// CRITICAL: Clear default claim type mappings to prevent JWT handler from transforming claims
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

// Configure Serilog
Serilog.Log.Logger = new Serilog.LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(new Serilog.Formatting.Compact.CompactJsonFormatter(), "logs/api-log.json", rollingInterval: Serilog.RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();

// Add Infrastructure services (DbContext, Identity, Services)
builder.Services.AddInfrastructure(builder.Configuration);

// JWT Authentication
var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") 
             ?? builder.Configuration["Jwt:Key"];

// Only validate in production - development can use appsettings key
if (builder.Environment.IsProduction() && 
    (string.IsNullOrEmpty(jwtKey) || jwtKey == "REPLACE_WITH_ENV_VARIABLE_IN_PRODUCTION"))
{
    throw new InvalidOperationException(
        "JWT Secret Key is not configured for production. Set JWT_SECRET_KEY environment variable.");
}

if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException(
        "JWT Secret Key is not configured. Check appsettings.Development.json or set JWT_SECRET_KEY environment variable.");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey)),
        RoleClaimType = ClaimTypes.Role
    };
});

// Swagger/OpenAPI - Enhanced Documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Restaurant App API", 
        Version = "v1.0",
        Description = @"
            **Food Ordering System API**
            
            Complete RESTful API for restaurant management and food ordering.
            
            **Features:**
            - User Authentication & Authorization
            - Menu Management (Categories & Items)
            - Order Processing & Tracking
            - Branch Management
            - Delivery System
            - Loyalty Points & Rewards
            - Reviews & Ratings
            - Offers & Coupons
            
            **Security:** All admin endpoints require JWT authentication with Admin role.
        ",
        Contact = new OpenApiContact
        {
            Name = "Restaurant App Support",
            Email = "support@restaurantapp.com"
        },
        License = new OpenApiLicense
        {
            Name = "MIT License"
        }
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

// CORS - Environment-based configuration
builder.Services.AddCors(options =>
{
    // Development: Allow all for easier testing
    options.AddPolicy("Development", policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
    
    // Production: Restrict to known origins
    options.AddPolicy("Production", policy =>
    {
        var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() 
                           ?? new[] { "http://localhost:5119" };
        
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 2,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst
            }));
            
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

// SignalR for real-time updates
builder.Services.AddSignalR();
builder.Services.AddScoped<RestaurantApp.API.Services.IAdminNotificationService, RestaurantApp.API.Services.AdminNotificationService>();
builder.Services.AddScoped<RestaurantApp.Application.Interfaces.IOrderNotificationService, RestaurantApp.API.Services.OrderNotificationService>();

// Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<RestaurantApp.Infrastructure.Data.ApplicationDbContext>();

var app = builder.Build();

// Log JWT configuration
app.Logger.LogInformation("JWT Config - Issuer: {Issuer}", builder.Configuration["Jwt:Issuer"]);
app.Logger.LogInformation("JWT Config - Audience: {Audience}", builder.Configuration["Jwt:Audience"]);
var sensitiveKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? builder.Configuration["Jwt:Key"];
var keyDisplay = string.IsNullOrEmpty(sensitiveKey) ? "NULL" : $"{sensitiveKey.Substring(0, Math.Min(5, sensitiveKey.Length))}...";
app.Logger.LogInformation("JWT Config - Key Used: {Key}", keyDisplay);

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurant App API v1");
    });
}

// Request/Response logging for monitoring
app.UseMiddleware<RestaurantApp.API.Middleware.RequestResponseLoggingMiddleware>();

// Global exception handling - must be early in pipeline
app.UseMiddleware<RestaurantApp.API.Middleware.ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseStaticFiles(); // Enable static tools (images, etc)

// Use environment-specific CORS policy
var corsPolicy = app.Environment.IsDevelopment() ? "Development" : "Production";
app.UseCors(corsPolicy);

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Map SignalR Hubs
app.MapHub<OrderHub>("/hubs/orders");

// Health Check Endpoints
app.MapHealthChecks("/health");

// Seed initial data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
        try
        {
            logger.LogInformation("Starting database seeding...");
            await DbInitializer.InitializeAsync(services);
            logger.LogInformation("Database seeding completed successfully!");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
            // Don't throw - allow the application to continue
        }
}

app.Run();
