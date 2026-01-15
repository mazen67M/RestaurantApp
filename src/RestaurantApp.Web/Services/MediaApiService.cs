using System.Net.Http.Json;

namespace RestaurantApp.Web.Services;

public class MediaApiService : BaseApiService
{
    public MediaApiService(HttpClient httpClient, Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider authProvider) 
        : base(httpClient, authProvider)
    {
    }

    public async Task<string?> UploadImageAsync(Stream fileStream, string fileName)
    {
        try
        {
            await EnsureAuthHeaderAsync();
            using var content = new MultipartFormDataContent();
            using var streamContent = new StreamContent(fileStream);
            content.Add(streamContent, "file", fileName);

            var response = await HttpClient.PostAsync("/api/media/upload", content);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<UploadResponse>();
                return result?.ImageUrl;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Image upload failed: {ex.Message}");
        }
        return null;
    }

    private class UploadResponse
    {
        public string ImageUrl { get; set; } = "";
    }
}
