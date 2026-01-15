using RestaurantApp.Application.Common;
using RestaurantApp.Application.DTOs.Offer;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestaurantApp.Application.Interfaces;

public interface IOfferService
{
    Task<ApiResponse<PagedResponse<OfferDto>>> GetOffersAsync(int page = 1, int pageSize = 20);
    Task<ApiResponse<List<OfferDto>>> GetActiveOffersAsync();
    Task<ApiResponse<OfferValidationResult>> ValidateCouponAsync(string code, decimal orderTotal, int? branchId = null, int? categoryId = null);
    Task<ApiResponse<OfferDto>> CreateOfferAsync(CreateOfferRequest request);
    Task<ApiResponse> UpdateOfferAsync(int id, UpdateOfferRequest request);
    Task<ApiResponse> DeleteOfferAsync(int id);
    Task<ApiResponse<bool>> ToggleOfferAsync(int id);
}
