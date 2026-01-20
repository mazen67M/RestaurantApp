using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

namespace RestaurantApp.API.Swagger;

/// <summary>
/// Swagger examples for Order DTOs
/// </summary>
public class CreateOrderDtoExample : IExamplesProvider<RestaurantApp.Application.DTOs.Order.CreateOrderDto>
{
    public RestaurantApp.Application.DTOs.Order.CreateOrderDto GetExamples()
    {
        return new RestaurantApp.Application.DTOs.Order.CreateOrderDto(
            BranchId: 1,
            OrderType: RestaurantApp.Domain.Enums.OrderType.Delivery,
            AddressId: 1,
            DeliveryAddressLine: "123 Main Street, Apartment 4B, Dubai",
            DeliveryLatitude: (decimal?)25.2048m,
            DeliveryLongitude: (decimal?)55.2708m,
            DeliveryNotes: "Please call when you arrive",
            RequestedDeliveryTime: DateTime.UtcNow.AddHours(2),
            CustomerNotes: "Extra napkins please",
            CouponCode: "SAVE20",
            Items: new List<RestaurantApp.Application.DTOs.Order.CreateOrderItemDto>
            {
                new(
                    MenuItemId: 1,
                    Quantity: 2,
                    Notes: "No onions",
                    AddOnIds: new List<int> { 1, 2 }
                ),
                new(
                    MenuItemId: 3,
                    Quantity: 1,
                    Notes: null,
                    AddOnIds: null
                )
            }
        );
    }
}

public class OrderCreatedDtoExample : IExamplesProvider<RestaurantApp.Application.Common.ApiResponse<RestaurantApp.Application.DTOs.Order.OrderCreatedDto>>
{
    public RestaurantApp.Application.Common.ApiResponse<RestaurantApp.Application.DTOs.Order.OrderCreatedDto> GetExamples()
    {
        return RestaurantApp.Application.Common.ApiResponse<RestaurantApp.Application.DTOs.Order.OrderCreatedDto>
            .SuccessResponse(new RestaurantApp.Application.DTOs.Order.OrderCreatedDto(
                OrderId: 123,
                OrderNumber: "ORD-20260120-A1B2C3",
                Total: 145.50m,
                EstimatedDeliveryTime: DateTime.UtcNow.AddMinutes(45)
            ));
    }
}

public class RegisterDtoExample : IExamplesProvider<RestaurantApp.Application.DTOs.Auth.RegisterDto>
{
    public RestaurantApp.Application.DTOs.Auth.RegisterDto GetExamples()
    {
        return new RestaurantApp.Application.DTOs.Auth.RegisterDto(
            Email: "customer@example.com",
            Password: "SecurePass123!",
            FullName: "Ahmed Mohammed",
            Phone: "+971501234567"
        );
    }
}

public class LoginDtoExample : IExamplesProvider<RestaurantApp.Application.DTOs.Auth.LoginDto>
{
    public RestaurantApp.Application.DTOs.Auth.LoginDto GetExamples()
    {
        return new RestaurantApp.Application.DTOs.Auth.LoginDto(
            Email: "customer@example.com",
            Password: "SecurePass123!"
        );
    }
}

public class ValidationProblemDetailsExample : IExamplesProvider<Microsoft.AspNetCore.Mvc.ValidationProblemDetails>
{
    public Microsoft.AspNetCore.Mvc.ValidationProblemDetails GetExamples()
    {
        return new Microsoft.AspNetCore.Mvc.ValidationProblemDetails
        {
            Title = "Validation failed",
            Status = 400,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Instance = "/api/orders",
            Errors = new Dictionary<string, string[]>
            {
                { "error0", new[] { "Branch ID is required" } },
                { "error1", new[] { "Order must contain at least one item" } }
            },
            Extensions = { { "traceId", "00-abc123-def456-01" } }
        };
    }
}

public class NotFoundProblemDetailsExample : IExamplesProvider<Microsoft.AspNetCore.Mvc.ProblemDetails>
{
    public Microsoft.AspNetCore.Mvc.ProblemDetails GetExamples()
    {
        return new Microsoft.AspNetCore.Mvc.ProblemDetails
        {
            Title = "Order not found",
            Detail = "The requested order with ID 999 does not exist",
            Status = 404,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            Instance = "/api/orders/999",
            Extensions = { { "traceId", "00-abc123-def456-01" } }
        };
    }
}
