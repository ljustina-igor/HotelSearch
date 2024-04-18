using HotelSearch.BL.Mappers;
using HotelSearch.Core;
using HotelSearch.Entities;
using NetTopologySuite.Geometries;

namespace HotelSearch.Services.UnitTests.Mappers;

public class HotelToHotelDtoMapperTests
{
    private static readonly GeometryFactory GeometryFactory =
        NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(HotelSearchConsts.Srid);

    [Fact]
    public void ToHotelDto_GivenNull_ReturnsNull()
    {
        // Arrange

        // Act
        var result = HotelToHotelDtoMapper.ToHotelDto(null);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ToHotelDto_GivenHotel_ReturnsCorrectlyMappedDto()
    {
        // Arrange
        var hotel = new Hotel
        {
            Id = Guid.NewGuid(),
            Name = "Test hotel",
            Price = 1,
            Coordinates = GeometryFactory.CreatePoint(new Coordinate(1, 3))
        };

        // Act
        var result = HotelToHotelDtoMapper.ToHotelDto(hotel);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(hotel.Id, result.Id);
        Assert.Equal(hotel.Name, result.Name);
        Assert.Equal(hotel.Price, result.Price);
        Assert.Equal(hotel.Coordinates.X, result.Longitude);
        Assert.Equal(hotel.Coordinates.Y, result.Latitude);
    }
}
