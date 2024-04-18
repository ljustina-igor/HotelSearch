using HotelSearch.BL;
using HotelSearch.Core;
using HotelSearch.Core.Exceptions;
using HotelSearch.Core.Models.Requests.Hotel;
using HotelSearch.Core.Models.Responses.Hotel;
using HotelSearch.Core.Repositories;
using HotelSearch.Core.UnitOfWork;
using HotelSearch.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using NetTopologySuite.Geometries;

namespace HotelSearch.Services.UnitTests;

public class HotelServiceTests
{
    private readonly Mock<IHotelRepository> _hotelRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<HotelService>> _loggerMock;
    private readonly HotelService _sut;

    private static readonly GeometryFactory GeometryFactory =
        NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(HotelSearchConsts.Srid);

    public HotelServiceTests()
    {
        _hotelRepositoryMock = new Mock<IHotelRepository>(MockBehavior.Strict);
        _unitOfWorkMock = new Mock<IUnitOfWork>(MockBehavior.Strict);
        _loggerMock = new Mock<ILogger<HotelService>>(MockBehavior.Strict);

        _sut = new HotelService(_hotelRepositoryMock.Object, _unitOfWorkMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task FindByIdAsync_GivenHotelWithGivenIdDoesNotExist_ReturnsNull()
    {
        // Arrange
        var id = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;
        _hotelRepositoryMock.Setup(x => x.FindByIdAsync(id, cancellationToken))
            .ReturnsAsync((Hotel)null);

        // Act
        var result = await _sut.FindByIdAsync(id, cancellationToken);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task FindByIdAsync_GivenHotelWithGivenIdDoesExist_ReturnsHotel()
    {
        // Arrange
        var id = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;
        _hotelRepositoryMock.Setup(x => x.FindByIdAsync(id, cancellationToken))
            .ReturnsAsync(new Hotel
            {
                Id = id,
                Name = "Esplanda",
                Price = 1234,
                Coordinates = GeometryFactory.CreatePoint(new Coordinate(1, 2))
            });

        // Act
        var result = await _sut.FindByIdAsync(id, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Esplanda", result.Name);
        Assert.Equal(1234, result.Price);
        Assert.Equal(1, result.Longitude);
        Assert.Equal(2, result.Latitude);
    }

    [Fact]
    public async Task GetAllAsync_GivenRepositoryReturnsNull_ReturnsEmptyCollection()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        _hotelRepositoryMock.Setup(x => x.GetAllAsync(cancellationToken))
            .ReturnsAsync((List<Hotel>)null);

        // Act
        var result = await _sut.GetAllAsync(cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllHotels()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        _hotelRepositoryMock.Setup(x => x.GetAllAsync(cancellationToken))
            .ReturnsAsync(new List<Hotel>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Esplanda",
                    Price = 66,
                    Coordinates = GeometryFactory.CreatePoint(new Coordinate(1, 2))
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Sheraton",
                    Price = 123,
                    Coordinates = GeometryFactory.CreatePoint(new Coordinate(30, 21))
                }
            });

        // Act
        var result = await _sut.GetAllAsync(cancellationToken);

        // Assert
        Assert.NotNull(result);

        Assert.Equal(2, result.Count());

        var firstHotel = result.First();
        Assert.Equal("Esplanda", firstHotel.Name);
        Assert.Equal(66, firstHotel.Price);
        Assert.Equal(1, firstHotel.Longitude);
        Assert.Equal(2, firstHotel.Latitude);

        var secondHotel = result.ElementAt(1);
        Assert.Equal("Sheraton", secondHotel.Name);
        Assert.Equal(123, secondHotel.Price);
        Assert.Equal(30, secondHotel.Longitude);
        Assert.Equal(21, secondHotel.Latitude);
    }

    [Fact]
    public async Task CreateAsync_GivenRequestIsNull_ThrowsArgumentNullException()
    {
        // Act
        await Assert.ThrowsAsync<ArgumentNullException>(
            async () => await _sut.CreateAsync(null, CancellationToken.None));
    }

    [Fact]
    public async Task CreateAsync_GivenRequestIsNotNull_SavesEntity()
    {
        // Arrange
        var request = new CreateHotelRequest("Test", 120, 16, 26);
        var cancellationToken = CancellationToken.None;


        _hotelRepositoryMock.Setup(x => x.Create(It.Is<Hotel>(y =>
            y.Name == "Test" &&
            y.Price == 120 &&
            y.Coordinates.X == 26.0 &&
            y.Coordinates.Y == 16.0)));

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(cancellationToken))
            .Returns(Task.CompletedTask);
        _loggerMock.Setup(x => x.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(),
            It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));

        // Act
        var result = await _sut.CreateAsync(request, cancellationToken);

        // Assert
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(cancellationToken), Times.Once);
        _loggerMock.Verify(x => x.Log(
            It.IsAny<LogLevel>(),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((o, t) => string.Equals(HotelSearchConsts.HotelCreatedMessage, o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
            It.IsAny<Exception>(),
            It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_GivenRequestIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        var id = Guid.NewGuid();
        _hotelRepositoryMock.Setup(p => p.FindByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Hotel)null);

        // Act
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _sut.UpdateAsync(id, null, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateAsync_GivenHotelForGivenIdIsNull_ThrowsEntityNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();
        _hotelRepositoryMock.Setup(p => p.FindByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Hotel)null);

        // Act
        await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
            await _sut.UpdateAsync(id, new UpdateHotelRequest("a", 1, 1, 1), CancellationToken.None));
    }

    [Fact]
    public async Task UpdateAsync_GivenHotelExists_UpdatesHotel()
    {
        // Arrange
        var id = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;
        var hotel = new Hotel
        {
            Id = id,
            Name = "A",
            Price = 1000,
            Coordinates = GeometryFactory.CreatePoint(new Coordinate(1, 2))
        };

        _hotelRepositoryMock.Setup(p => p.FindByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(hotel);

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(cancellationToken))
            .Returns(Task.CompletedTask);
        _loggerMock.Setup(x => x.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(),
            It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));

        var request = new UpdateHotelRequest("Hotel", 11, 13, 12);

        // Act
        await _sut.UpdateAsync(id, request, CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(cancellationToken), Times.Once);
        _loggerMock.Verify(x => x.Log(
            It.IsAny<LogLevel>(),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((o, t) => string.Equals(HotelSearchConsts.HotelUpdatedMessage, o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
            It.IsAny<Exception>(),
            It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        Assert.Equal(request.Name, hotel.Name);
        Assert.Equal(request.Price, hotel.Price);
        Assert.Equal(request.Longitude, hotel.Coordinates.X);
        Assert.Equal(request.Latitude, hotel.Coordinates.Y);
    }

    [Fact]
    public async Task DeleteAsync_GivenHotelForGivenIdIsNull_ThrowsEntityNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();
        _hotelRepositoryMock.Setup(p => p.FindByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Hotel)null);

        // Act
        await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
            await _sut.DeleteAsync(id, CancellationToken.None));
    }

    [Fact]
    public async Task DeleteAsync_GivenhotelExists_DeletesHotel()
    {
        // Arrange
        var id = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;
        var hotel = new Hotel
        {
            Id = id,
            Name = "A",
            Price = 1000,
            Coordinates = GeometryFactory.CreatePoint(new Coordinate(1, 2))
        };

        _hotelRepositoryMock.Setup(p => p.FindByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(hotel);

        _hotelRepositoryMock.Setup(p => p.Delete(hotel));

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(cancellationToken))
            .Returns(Task.CompletedTask);
        _loggerMock.Setup(x => x.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(),
            It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));

        // Act
        await _sut.DeleteAsync(id, CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(cancellationToken), Times.Once);
        _loggerMock.Verify(x => x.Log(
            It.IsAny<LogLevel>(),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((o, t) => string.Equals(HotelSearchConsts.HotelDeletedMessage, o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
            It.IsAny<Exception>(),
            It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
    }

    [Fact]
    public async Task FindLocationsOrderedByPriceAndDistanceAsync_GivenLongitudeAndLatitude_ReturnsHotels()
    {
        // Arrange
        var longitude = 11.0;
        var latitude = 25.0;
        var cancellationToken = CancellationToken.None;
        var skip = 0;
        var take = 15;

        var hotelsWithDistanceList = new List<HotelWithDistanceDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Esplanda",
                Price = 66,
                DistanceInMeters = 5223
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Sheraton",
                Price = 123,
                DistanceInMeters = 1254
            }
        };

        _hotelRepositoryMock.Setup(p =>
                p.FindCheapestAndClosestToCoordinates(It.Is<Point>(i => i.X == longitude && i.Y == latitude), skip, take,
                    cancellationToken))
            .ReturnsAsync(hotelsWithDistanceList);

        // Act
        var result = await _sut.FindLocationsOrderedByPriceAndDistanceAsync(longitude, latitude, skip, take, cancellationToken);

        // Assert
        Assert.Equal(hotelsWithDistanceList, result);
    }

    [Fact]
    public async Task FindLocationsOrderedByPriceAndDistanceAsync_GivenLongitudeAndLatitudeAndSkip_ReturnsLessHotels()
    {
        // Arrange
        var longitude = 11.0;
        var latitude = 25.0;
        var cancellationToken = CancellationToken.None;
        var skip = 1;
        var take = 15;

        var hotelsWithDistanceList = new List<HotelWithDistanceDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Esplanda",
                Price = 66,
                DistanceInMeters = 5223
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Sheraton",
                Price = 123,
                DistanceInMeters = 1254
            }
        };

        var hotelsFiltered = hotelsWithDistanceList.Skip(skip).Take(take);

        _hotelRepositoryMock.Setup(p =>
                p.FindCheapestAndClosestToCoordinates(It.Is<Point>(i => i.X == longitude && i.Y == latitude), skip, take,
                    cancellationToken))
            .ReturnsAsync(hotelsFiltered);

        // Act
        var result = await _sut.FindLocationsOrderedByPriceAndDistanceAsync(longitude, latitude, skip, take, cancellationToken);

        // Assert
        Assert.NotEqual(hotelsWithDistanceList.Count, result.Count());
    }

}
