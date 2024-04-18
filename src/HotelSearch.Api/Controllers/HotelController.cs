using HotelSearch.Core.Models.Requests.Hotel;
using HotelSearch.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using HotelSearch.Core.Models.Responses.Hotel;

namespace HotelSearch.Controllers;

/// <summary>
/// Controller for hotel management
/// </summary>
[ApiController]
[Route("[controller]")]
public class HotelController : ControllerBase
{
    private readonly IHotelService _hotelService;

    public HotelController(IHotelService hotelService)
    {
        _hotelService = hotelService;
    }

    /// <summary>
    /// Gets all hotels
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>Collection of <see cref="HotelDto"/></returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<HotelDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
    {
        var data = await _hotelService.GetAllAsync(cancellationToken);
        return Ok(data);
    }

    /// <summary>
    /// Returns hotel by given id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns <see cref="HotelDto"/> instance</returns>
    [HttpGet("/{id:guid}")]
    [ProducesResponseType(typeof(HotelDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> FindByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var data = await _hotelService.FindByIdAsync(id, cancellationToken);

        if (data == null)
        {
            return NotFound();
        }

        return Ok(data);
    }

    /// <summary>
    /// Creates new hotel
    /// </summary>
    /// <param name="request">Hotel data to add.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns <see cref="HotelDto"/> instance of created hotel</returns>
    [HttpPost]
    [ProducesResponseType(typeof(HotelDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateHotelRequest request,
        CancellationToken cancellationToken)
    {
        var hotel = await _hotelService.CreateAsync(request, cancellationToken);
        return Ok(hotel);
    }

    /// <summary>
    /// Updates hotel
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request">Hotel data to update</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns <see cref="HotelDto"/> instance of updated hotel</returns>
    [HttpPut("/{id:guid}")]
    [ProducesResponseType(typeof(HotelDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateHotelRequest request,
        CancellationToken cancellationToken)
    {
        var hotel = await _hotelService.UpdateAsync(id, request, cancellationToken);
        return Ok(hotel);
    }

    /// <summary>
    /// Deletes hotel
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    [HttpDelete("/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        await _hotelService.DeleteAsync(id, cancellationToken);
        return Ok();
    }

    /// <summary>
    /// Gets all hotels ordered by cheapest and closest first for provided location.
    /// </summary>
    /// <param name="longitude">longitude to be used for hotel ordering by distance</param>
    /// <param name="latitude">latitude to be used for hotel ordering by distance</param>
    /// <param name="cancellationToken"></param>
    /// <param name="skip">Number of hotels to skip during search</param>
    /// <param name="take">Number of hotels to retrieve</param>
    /// <returns>Collection of <see cref="HotelWithDistanceDto"/></returns>
    [HttpGet("/find")]
    [ProducesResponseType(typeof(IEnumerable<HotelWithDistanceDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> FindByLowestPriceAndDistanceAsync([
            FromQuery, Range(-180, 180, MinimumIsExclusive = false, MaximumIsExclusive = false)]
        double longitude, [
            FromQuery,  Range(-90, 90, MinimumIsExclusive = false, MaximumIsExclusive = false)]
        double latitude, CancellationToken cancellationToken, int skip = 0, int take = 15)
    {
        var hotels =
            await _hotelService.FindLocationsOrderedByPriceAndDistanceAsync(longitude, latitude, skip, take, cancellationToken);
        return Ok(hotels);
    }
}
