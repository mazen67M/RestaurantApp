using RestaurantApp.Application.Interfaces;

namespace RestaurantApp.API.Middleware;

public class TokenBlacklistMiddleware
{
    private readonly RequestDelegate _next;

    public TokenBlacklistMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITokenBlacklistService blacklistService)
    {
        var authHeader = context.Request.Headers["Authorization"].ToString();
        
        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            var token = authHeader.Substring("Bearer ".Length).Trim();
            
            if (await blacklistService.IsBlacklistedAsync(token))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { success = false, message = "Token has been revoked" });
                return;
            }
        }

        await _next(context);
    }
}

public static class TokenBlacklistMiddlewareExtensions
{
    public static IApplicationBuilder UseTokenBlacklist(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<TokenBlacklistMiddleware>();
    }
}
