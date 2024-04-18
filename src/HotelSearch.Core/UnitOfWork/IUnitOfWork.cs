namespace HotelSearch.Core.UnitOfWork;

public interface IUnitOfWork
{
    /// <summary>
    /// Saves all changes.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task SaveChangesAsync(CancellationToken cancellationToken);
}
