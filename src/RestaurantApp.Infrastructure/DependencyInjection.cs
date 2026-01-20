using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestaurantApp.Application.Interfaces;
using RestaurantApp.Domain.Entities;
using RestaurantApp.Infrastructure.Data;
using RestaurantApp.Infrastructure.Services;

namespace RestaurantApp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        // Identity
        services.AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 6;
            
            options.User.RequireUniqueEmail = true;
            
            // PRODUCTION SECURITY: Email confirmation now REQUIRED
            // Users must verify their email before they can sign in
            options.SignIn.RequireConfirmedEmail = true;
            
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        // Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IRestaurantService, RestaurantService>();
        services.AddScoped<IMenuService, MenuService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IAddressService, AddressService>();
        services.AddScoped<IEmailService, EmailService>();
        
        // Phase 3 Services
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<ILoyaltyService, LoyaltyService>();
        
        // Delivery Management
        services.AddScoped<IDeliveryService, DeliveryService>();
        
        // Admin Services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IOfferService, OfferService>();
        
        // Security Services
        services.AddScoped<IResourceAuthorizationService, ResourceAuthorizationService>();
        
        // Performance Services
        services.AddScoped<ICacheService, CacheService>();
        
        // Security: Token Blacklist
        services.AddScoped<ITokenBlacklistService, TokenBlacklistService>();


        return services;
    }
}
