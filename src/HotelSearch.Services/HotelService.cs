using HotelSearch.BL.Mappers;
using HotelSearch.Core;
using HotelSearch.Core.Exceptions;
using HotelSearch.Core.Models.Requests.Hotel;
using HotelSearch.Core.Models.Responses.Hotel;
using HotelSearch.Core.Repositories;
using HotelSearch.Core.Services;
using HotelSearch.Core.UnitOfWork;
using HotelSearch.Entities;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;

namespace HotelSearch.BL;

public class HotelService : IHotelService
{
    private readonly IHotelRepository _hotelRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<HotelService> _logger;

    private static readonly GeometryFactory GeometryFactory =
        NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(HotelSearchConsts.Srid);

    public HotelService(
        IHotelRepository hotelRepository,
        IUnitOfWork unitOfWork,
        ILogger<HotelService> logger)
    {
        _hotelRepository = hotelRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<HotelDto> FindByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var hotel = await _hotelRepository.FindByIdAsync(id, cancellationToken);

        if (hotel == null)
        {
            return null;
        }

        return HotelToHotelDtoMapper.ToHotelDto(hotel);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<HotelDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var hotels = await _hotelRepository.GetAllAsync(cancellationToken);

        if (hotels == null)
        {
            return Array.Empty<HotelDto>();
        }

        return hotels.Select(HotelToHotelDtoMapper.ToHotelDto);
    }

    /// <inheritdoc />
    public async Task<HotelDto> CreateAsync(CreateHotelRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var hotel = new Hotel
        {
            Name = request.Name,
            Price = request.Price,
            Coordinates = GeometryFactory.CreatePoint(new Coordinate(request.Longitude, request.Latitude))
        };

        _hotelRepository.Create(hotel);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(HotelSearchConsts.HotelCreatedMessage);

        return HotelToHotelDtoMapper.ToHotelDto(hotel);
    }

    /// <inheritdoc />
    public async Task<HotelDto> UpdateAsync(Guid id, UpdateHotelRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var hotel = await _hotelRepository.FindByIdAsync(id, cancellationToken);
        EntityNotFoundException.ThrowIfNull(hotel);

        hotel.Name = request.Name;
        hotel.Price = request.Price;
        hotel.Coordinates = GeometryFactory.CreatePoint(new Coordinate(request.Longitude, request.Latitude));

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(HotelSearchConsts.HotelUpdatedMessage);

        return HotelToHotelDtoMapper.ToHotelDto(hotel);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var hotel = await _hotelRepository.FindByIdAsync(id, cancellationToken);
        EntityNotFoundException.ThrowIfNull(hotel);

        _hotelRepository.Delete(hotel);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(HotelSearchConsts.HotelDeletedMessage);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<HotelWithDistanceDto>> FindLocationsOrderedByPriceAndDistanceAsync(double longitude,
        double latitude, int skip, int take, CancellationToken cancellationToken)
    {
        var point = GeometryFactory.CreatePoint(new Coordinate(longitude, latitude));

        return await _hotelRepository.FindCheapestAndClosestToCoordinates(point, skip, take, cancellationToken);
    }
}
