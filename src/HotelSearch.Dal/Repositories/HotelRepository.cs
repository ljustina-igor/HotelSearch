using HotelSearch.Core.Models.Responses.Hotel;
using HotelSearch.Core.Repositories;
using HotelSearch.Entities;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace HotelSearch.Dal.Repositories;

public class HotelRepository : IHotelRepository
{
    private readonly HotelSearchDbContext _context;

    public HotelRepository(HotelSearchDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public void Create(Hotel hotel)
    {
        _context.Add(hotel);
    }

    /// <inheritdoc />
    public async Task<Hotel> FindByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Hotels.FindAsync([id], cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Hotel>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Hotels.ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public void Delete(Hotel hotel)
    {
        _context.Remove(hotel);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<HotelWithDistanceDto>> FindCheapestAndClosestToCoordinates(Point point, int skip, int take, CancellationToken cancellationToken)
    {
        return await _context.Hotels
        .Select(x => new HotelWithDistanceDto
        {
            Id = x.Id,
            Name = x.Name,
            Price = x.Price,
            DistanceInMeters = x.Coordinates.Distance(point)
        })
        .OrderBy(x => x.Price)
        .ThenBy(x => x.DistanceInMeters)
        .Skip(skip)
        .Take(take)
        .ToListAsync(cancellationToken);
    }
}