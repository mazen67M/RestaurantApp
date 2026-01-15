namespace RestaurantApp.API.Middleware;

/// <summary>
/// Middleware to log HTTP requests and responses for monitoring and debugging
/// </summary>
public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

    public RequestResponseLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestResponseLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Log request
        var requestTime = DateTime.UtcNow;
        var requestPath = context.Request.Path;
        var requestMethod = context.Request.Method;
        var requestQuery = context.Request.QueryString.ToString();

        _logger.LogInformation(
            "HTTP {Method} {Path}{Query} started at {Time}",
            requestMethod,
            requestPath,
            requestQuery,
            requestTime);

        // Process request
        var originalBodyStream = context.Response.Body;
        
        try
        {
            await _next(context);
        }
        finally
        {
            // Log response
            var responseTime = DateTime.UtcNow;
            var duration = (responseTime - requestTime).TotalMilliseconds;
            var statusCode = context.Response.StatusCode;

            var logLevel = statusCode >= 500 ? LogLevel.Error :
                          statusCode >= 400 ? LogLevel.Warning :
                          LogLevel.Information;

            _logger.Log(
                logLevel,
                "HTTP {Method} {Path}{Query} responded {StatusCode} in {Duration}ms",
                requestMethod,
                requestPath,
                requestQuery,
                statusCode,
                duration);
        }
    }
}
