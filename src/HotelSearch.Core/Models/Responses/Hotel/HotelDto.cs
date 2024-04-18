namespace HotelSearch.Core.Models.Responses.Hotel;

public record HotelDto(
    Guid Id,
    string Name,
    decimal Price,
    double Latitude,
    double Longitude);
