using Microsoft.AspNetCore.Mvc;

namespace RestaurantApp.Application.Common;

/// <summary>
/// Factory for creating RFC 7807 ProblemDetails responses
/// </summary>
public static class ProblemDetailsFactory
{
    /// <summary>
    /// Create a validation problem details response
    /// </summary>
    public static ValidationProblemDetails CreateValidationProblem(
        string title,
        List<string> errors,
        string? instance = null)
    {
        var problemDetails = new ValidationProblemDetails
        {
            Title = title,
            Status = 400,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Instance = instance
        };

        for (int i = 0; i < errors.Count; i++)
        {
            problemDetails.Errors.Add($"error{i}", new[] { errors[i] });
        }

        return problemDetails;
    }

    /// <summary>
    /// Create a not found problem details response
    /// </summary>
    public static ProblemDetails CreateNotFoundProblem(
        string title,
        string detail,
        string? instance = null)
    {
        return new ProblemDetails
        {
            Title = title,
            Detail = detail,
            Status = 404,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            Instance = instance
        };
    }

    /// <summary>
    /// Create an unauthorized problem details response
    /// </summary>
    public static ProblemDetails CreateUnauthorizedProblem(
        string title = "Unauthorized",
        string? detail = null,
        string? instance = null)
    {
        return new ProblemDetails
        {
            Title = title,
            Detail = detail ?? "You are not authorized to access this resource",
            Status = 401,
            Type = "https://tools.ietf.org/html/rfc7235#section-3.1",
            Instance = instance
        };
    }

    /// <summary>
    /// Create a forbidden problem details response
    /// </summary>
    public static ProblemDetails CreateForbiddenProblem(
        string title = "Forbidden",
        string? detail = null,
        string? instance = null)
    {
        return new ProblemDetails
        {
            Title = title,
            Detail = detail ?? "You do not have permission to access this resource",
            Status = 403,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3",
            Instance = instance
        };
    }

    /// <summary>
    /// Create a conflict problem details response
    /// </summary>
    public static ProblemDetails CreateConflictProblem(
        string title,
        string detail,
        string? instance = null)
    {
        return new ProblemDetails
        {
            Title = title,
            Detail = detail,
            Status = 409,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8",
            Instance = instance
        };
    }

    /// <summary>
    /// Create an internal server error problem details response
    /// </summary>
    public static ProblemDetails CreateInternalServerErrorProblem(
        string title = "Internal Server Error",
        string? detail = null,
        string? instance = null)
    {
        return new ProblemDetails
        {
            Title = title,
            Detail = detail ?? "An unexpected error occurred while processing your request",
            Status = 500,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            Instance = instance
        };
    }

    /// <summary>
    /// Create a bad request problem details response
    /// </summary>
    public static ProblemDetails CreateBadRequestProblem(
        string title,
        string detail,
        string? instance = null)
    {
        return new ProblemDetails
        {
            Title = title,
            Detail = detail,
            Status = 400,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Instance = instance
        };
    }

    /// <summary>
    /// Create a problem details response from an exception
    /// </summary>
    public static ProblemDetails CreateExceptionProblem(
        Exception exception,
        string? instance = null,
        bool includeDetails = false)
    {
        return exception switch
        {
            UnauthorizedAccessException => CreateUnauthorizedProblem(
                instance: instance,
                detail: includeDetails ? exception.Message : null),

            ArgumentException or ArgumentNullException => CreateBadRequestProblem(
                title: "Invalid Request",
                detail: includeDetails ? exception.Message : "Invalid request parameters",
                instance: instance),

            KeyNotFoundException => CreateNotFoundProblem(
                title: "Resource Not Found",
                detail: includeDetails ? exception.Message : "The requested resource was not found",
                instance: instance),

            InvalidOperationException => CreateBadRequestProblem(
                title: "Invalid Operation",
                detail: includeDetails ? exception.Message : "The requested operation is not valid",
                instance: instance),

            _ => CreateInternalServerErrorProblem(
                instance: instance,
                detail: includeDetails ? exception.Message : null)
        };
    }
}
