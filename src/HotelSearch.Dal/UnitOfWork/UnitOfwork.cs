using HotelSearch.Core.UnitOfWork;

namespace HotelSearch.Dal.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly HotelSearchDbContext _context;

    public UnitOfWork(HotelSearchDbContext context)
    {
        _context = context;
    }

    /// <inhertidoc />
    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
