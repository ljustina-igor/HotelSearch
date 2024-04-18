using HotelSearch.Controllers;
using HotelSearch.Core.Models.Requests.Hotel;
using HotelSearch.Core.Models.Responses.Hotel;
using HotelSearch.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HotelSearch.Api.UnitTests.Controllers;

public class HotelControllerTests
{
    private readonly Mock<IHotelService> _hotelServiceMock;
    private readonly HotelController _sut;

    public HotelControllerTests()
    {
        _hotelServiceMock = new Mock<IHotelService>(MockBehavior.Strict);

        _sut = new HotelController(_hotelServiceMock.Object);
    }

    [Fact]
    public async Task FindByIdAsync_WhenRequestedHotelDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;
        _hotelServiceMock.Setup(x => x.FindByIdAsync(id, cancellationToken))
            .ReturnsAsync((HotelDto)null);


        // Act
        var result = await _sut.FindByIdAsync(id, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task FindByIdAsync_WhenRequestedHotelDoesExist_ReturnsOk()
    {
        // Arrange
        var id = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        var hotel = new HotelDto(id, "Test", 1, 2, 3);
        _hotelServiceMock.Setup(x => x.FindByIdAsync(id, cancellationToken))
            .ReturnsAsync(hotel);


        // Act
        var result = await _sut.FindByIdAsync(id, cancellationToken);

        // Assert
        Assert.NotNull(result);
        var okResult = Assert.IsType<OkObjectResult>(result);
        var data = Assert.IsType<HotelDto>(okResult.Value);

        Assert.Equal(hotel, data);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOk()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        var hotel1 = new HotelDto(Guid.NewGuid(), "Test", 1, 2, 3);
        var hotel2 = new HotelDto(Guid.NewGuid(), "Test2", 12, 22, 33);
        _hotelServiceMock.Setup(x => x.GetAllAsync(cancellationToken))
            .ReturnsAsync(new List<HotelDto> { hotel1, hotel2 });


        // Act
        var result = await _sut.GetAllAsync(cancellationToken);

        // Assert
        Assert.NotNull(result);
        var okResult = Assert.IsType<OkObjectResult>(result);
        var data = Assert.IsAssignableFrom<IEnumerable<HotelDto>>(okResult.Value);

        Assert.Equal(2, data.Count());
        Assert.Contains(data, x => x == hotel1);
        Assert.Contains(data, x => x == hotel2);
    }

    [Fact]
    public async Task CreateAsync_ReturnsOk()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        var hotel = new HotelDto(Guid.NewGuid(), "Test", 1, 2, 3);
        var createRequest = new CreateHotelRequest("Test", 1, 2, 3);

        _hotelServiceMock.Setup(x => x.CreateAsync(createRequest, cancellationToken))
            .ReturnsAsync(hotel);


        // Act
        var result = await _sut.CreateAsync(createRequest, cancellationToken);

        // Assert
        Assert.NotNull(result);
        var okResult = Assert.IsType<OkObjectResult>(result);
        var data = Assert.IsType<HotelDto>(okResult.Value);

        Assert.Equal(hotel, data);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsOk()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        var id = Guid.NewGuid();
        var hotel = new HotelDto(Guid.NewGuid(), "Test", 1, 2, 3);
        var updateHotelRequest = new UpdateHotelRequest("Test", 1, 2, 3);

        _hotelServiceMock.Setup(x => x.UpdateAsync(id, updateHotelRequest, cancellationToken))
            .ReturnsAsync(hotel);

        // Act
        var result = await _sut.UpdateAsync(id, updateHotelRequest, cancellationToken);

        // Assert
        Assert.NotNull(result);
        var okResult = Assert.IsType<OkObjectResult>(result);
        var data = Assert.IsType<HotelDto>(okResult.Value);

        Assert.Equal(hotel, data);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsOk()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        var id = Guid.NewGuid();


        _hotelServiceMock.Setup(x => x.DeleteAsync(id, cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.DeleteAsync(id, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task FindByLowestPriceAndDistanceAsync_ReturnsOk()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var longitude = 1.0;
        var latitude = 2.0;
        var skip = 0;
        var take = 15;

        var hotel1 = new HotelWithDistanceDto
        {
            Id = Guid.NewGuid(),
            DistanceInMeters = 12,
            Name = "Hotel1",
            Price = 1
        };

        var hotel2 = new HotelWithDistanceDto
        {
            Id = Guid.NewGuid(),
            DistanceInMeters = 123,
            Name = "Hotel12",
            Price = 13
        };
        _hotelServiceMock.Setup(x =>
                x.FindLocationsOrderedByPriceAndDistanceAsync(longitude, latitude, skip, take, cancellationToken))
            .ReturnsAsync(new List<HotelWithDistanceDto> { hotel1, hotel2 });


        // Act
        var result = await _sut.FindByLowestPriceAndDistanceAsync(longitude, latitude, cancellationToken);

        // Assert
        Assert.NotNull(result);
        var okResult = Assert.IsType<OkObjectResult>(result);
        var data = Assert.IsAssignableFrom<IEnumerable<HotelWithDistanceDto>>(okResult.Value);

        Assert.Equal(2, data.Count());
        Assert.Contains(data, x => x == hotel1);
        Assert.Contains(data, x => x == hotel2);
    }
}
