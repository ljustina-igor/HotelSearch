using HotelSearch.Core.Models.Responses.Hotel;
using HotelSearch.Entities;
using NetTopologySuite.Geometries;

namespace HotelSearch.Core.Repositories;

public interface IHotelRepository
{
    /// <summary>
    /// Creates hotel.
    /// </summary>
    /// <param name="hotel"></param>
    void Create(Hotel hotel);

    /// <summary>
    /// Finds hotel by id.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken">cancellation token</param>
    /// <returns>Null if not found, otherwise instance of <see cref="Hotel"/> </returns>
    Task<Hotel> FindByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Get all hotels.
    /// </summary>
    /// <returns>Collection of all hotels</returns>
    Task<IEnumerable<Hotel>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Deletes given hotel.
    /// </summary>
    /// <param name="hotel">Hotel to be deleted</param>
    void Delete(Hotel hotel);

    /// <summary>
    /// Gets all hotels cheapest and closest first for provided location.
    /// </summary>
    /// <param name="point">Representation of current location by <see cref="Point"> object. Y is latitude, X is longitude.</param>
    /// <param name="skip">Number of hotels to skip during search</param>
    /// <param name="take">Number of hotels to retrieve</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Collection of all hotels ordered by cheapest and closest first.</returns>
    Task<IEnumerable<HotelWithDistanceDto>> FindCheapestAndClosestToCoordinates(Point point, int skip, int take, CancellationToken cancellationToken);
}