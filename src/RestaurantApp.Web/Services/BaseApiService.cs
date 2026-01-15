using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;

namespace RestaurantApp.Web.Services;

public abstract class BaseApiService
{
    protected readonly HttpClient HttpClient;
    protected readonly AuthenticationStateProvider AuthProvider;

    protected BaseApiService(HttpClient httpClient, AuthenticationStateProvider authProvider)
    {
        HttpClient = httpClient;
        AuthProvider = authProvider;
    }

    protected async Task EnsureAuthHeaderAsync()
    {
        var authState = await AuthProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user.Identity?.IsAuthenticated == true)
        {
            var token = user.FindFirst("AuthToken")?.Value;
            if (!string.IsNullOrEmpty(token))
            {
                HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                // System.Console.WriteLine($"Attached Bearer token for user: {user.Identity.Name}");
            }
            else
            {
                System.Console.WriteLine("Warning: User is authenticated but 'AuthToken' claim is missing.");
            }
        }
        else
        {
            System.Console.WriteLine("Warning: User is NOT authenticated in BaseApiService.");
        }
    }
}
