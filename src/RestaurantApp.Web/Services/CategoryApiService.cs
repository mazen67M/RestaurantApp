using System.Net.Http.Json;

namespace RestaurantApp.Web.Services;

public class CategoryApiService : BaseApiService
{
    public CategoryApiService(HttpClient httpClient, Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider authProvider) 
        : base(httpClient, authProvider)
    {
    }

    public async Task<List<CategoryDto>> GetCategoriesAsync()
    {
        try
        {
            await EnsureAuthHeaderAsync();
            // The actual API endpoint is /api/menu/categories
            var response = await HttpClient.GetAsync("/api/menu/categories");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<CategoryDto>>>();
                return result?.Data ?? new List<CategoryDto>();
            }
            Console.WriteLine($"Get categories failed: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching categories: {ex.Message}");
        }
        return new List<CategoryDto>();
    }

    public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
    {
        try
        {
            await EnsureAuthHeaderAsync();
            var response = await HttpClient.GetAsync($"/api/menu/categories/{id}");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<CategoryDto>>();
                return result?.Data;
            }
        }
        catch
        {
            return null;
        }
        return null;
    }

    public async Task<bool> CreateCategoryAsync(CategoryDto category)
    {
        try
        {
            await EnsureAuthHeaderAsync();
            var createDto = new
            {
                NameEn = category.NameEn,
                NameAr = category.NameAr,
                DescriptionEn = category.DescriptionEn,
                DescriptionAr = category.DescriptionAr,
                ImageUrl = category.ImageUrl,
                SortOrder = category.SortOrder,
                IsActive = category.IsActive
            };
            var response = await HttpClient.PostAsJsonAsync("/api/menu/categories", createDto);
            Console.WriteLine($"Create category response: {response.StatusCode}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating category: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> UpdateCategoryAsync(int id, CategoryDto category)
    {
        try
        {
            await EnsureAuthHeaderAsync();
            var updateDto = new
            {
                NameEn = category.NameEn,
                NameAr = category.NameAr,
                DescriptionEn = category.DescriptionEn,
                DescriptionAr = category.DescriptionAr,
                ImageUrl = category.ImageUrl,
                SortOrder = category.SortOrder,
                IsActive = category.IsActive
            };
            var response = await HttpClient.PutAsJsonAsync($"/api/menu/categories/{id}", updateDto);
            Console.WriteLine($"Update category response: {response.StatusCode}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating category: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        try
        {
            await EnsureAuthHeaderAsync();
            var response = await HttpClient.DeleteAsync($"/api/menu/categories/{id}");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}


public class CategoryDto
{
    public int Id { get; set; }
    public string NameEn { get; set; } = "";
    public string NameAr { get; set; } = "";
    public string? DescriptionEn { get; set; }
    public string? DescriptionAr { get; set; }
    public string? ImageUrl { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    public int ItemsCount { get; set; }
}
