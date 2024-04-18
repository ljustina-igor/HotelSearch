using HotelSearch.Core.Models.Responses.Hotel;
using HotelSearch.Entities;

namespace HotelSearch.BL.Mappers;

public static class HotelToHotelDtoMapper
{
    public static HotelDto ToHotelDto(Hotel hotel)
    {
        if (hotel == null)
        {
            return null;
        }
        
        return new HotelDto(hotel.Id, hotel.Name, hotel.Price, hotel.Coordinates.Y, hotel.Coordinates.X);
    }
}
