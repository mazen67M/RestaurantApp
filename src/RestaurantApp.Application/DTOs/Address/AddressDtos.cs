using System.ComponentModel.DataAnnotations;

namespace RestaurantApp.Application.DTOs.Address;

public record UserAddressDto(
    int Id,
    string Label,
    string AddressLine,
    string? BuildingName,
    string? Floor,
    string? Apartment,
    string? Landmark,
    decimal Latitude,
    decimal Longitude,
    bool IsDefault
);

public record CreateAddressDto(
    [property: Required(ErrorMessage = "Address label is required")]
    [property: StringLength(50, ErrorMessage = "Label cannot exceed 50 characters")]
    string Label,
    
    [property: Required(ErrorMessage = "Address line is required")]
    [property: StringLength(200, ErrorMessage = "Address line cannot exceed 200 characters")]
    string AddressLine,
    
    [property: StringLength(100, ErrorMessage = "Building name cannot exceed 100 characters")]
    string? BuildingName,
    
    [property: StringLength(20, ErrorMessage = "Floor cannot exceed 20 characters")]
    string? Floor,
    
    [property: StringLength(20, ErrorMessage = "Apartment cannot exceed 20 characters")]
    string? Apartment,
    
    [property: StringLength(200, ErrorMessage = "Landmark cannot exceed 200 characters")]
    string? Landmark,
    
    [property: Required(ErrorMessage = "Latitude is required")]
    [property: Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
    decimal Latitude,
    
    [property: Required(ErrorMessage = "Longitude is required")]
    [property: Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
    decimal Longitude,
    
    bool IsDefault = false
);

public record UpdateAddressDto(
    [property: Required(ErrorMessage = "Address label is required")]
    [property: StringLength(50, ErrorMessage = "Label cannot exceed 50 characters")]
    string Label,
    
    [property: Required(ErrorMessage = "Address line is required")]
    [property: StringLength(200, ErrorMessage = "Address line cannot exceed 200 characters")]
    string AddressLine,
    
    [property: StringLength(100, ErrorMessage = "Building name cannot exceed 100 characters")]
    string? BuildingName,
    
    [property: StringLength(20, ErrorMessage = "Floor cannot exceed 20 characters")]
    string? Floor,
    
    [property: StringLength(20, ErrorMessage = "Apartment cannot exceed 20 characters")]
    string? Apartment,
    
    [property: StringLength(200, ErrorMessage = "Landmark cannot exceed 200 characters")]
    string? Landmark,
    
    [property: Required(ErrorMessage = "Latitude is required")]
    [property: Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
    decimal Latitude,
    
    [property: Required(ErrorMessage = "Longitude is required")]
    [property: Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
    decimal Longitude,
    
    bool IsDefault
);
