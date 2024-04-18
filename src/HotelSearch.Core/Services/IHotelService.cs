using HotelSearch.Core.Models.Requests.Hotel;
using HotelSearch.Core.Models.Responses.Hotel;

namespace HotelSearch.Core.Services;

public interface IHotelService
{
    /// <summary>
    /// Finds hotel by id.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Null if not found, otherwise instance of <see cref="HotelDto"/> </returns>
    Task<HotelDto> FindByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Get all hotels.
    /// </summary>
    /// <returns>Collection of all hotels.</returns>
    Task<IEnumerable<HotelDto>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Deletes hotel by given id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Creates hotel.
    /// </summary>
    /// <param name="request">Instance of <see cref="CreateHotelRequest"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Instance of <see cref="HotelDto"> for created hotel</returns>
    /// <remarks>Will throw <see cref="ArgumentNullException"/> if request is null.</remarks>
    Task<HotelDto> CreateAsync(CreateHotelRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Updates hotel by given id.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request">Instance of <see cref="UpdateHotelRequest"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Instance of <see cref="HotelDto"> for updated hotel</returns>
    /// <remarks>Will throw <see cref="ArgumentNullException"/> if request is null.</remarks>
    /// <remarks>Will throw <see cref="EntityNotFoundException"/> if hotel is null.</remarks>
    Task<HotelDto> UpdateAsync(Guid id, UpdateHotelRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Gets all hotels ordered by cheapest and closest first for provided location.
    /// </summary>
    /// <param name="longitude">longitude to be used for hotel ordering by distance</param>
    /// <param name="latitude">latitude to be used for hotel ordering by distance</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Collection of <see cref="HotelWithDistanceDto"> instances</returns>
    Task<IEnumerable<HotelWithDistanceDto>> FindLocationsOrderedByPriceAndDistanceAsync(double longitude,
        double latitude, int skip, int take, CancellationToken cancellationToken);
}
