namespace HotelSearch.Core.Exceptions;

public sealed class EntityNotFoundException : Exception
{
    public EntityNotFoundException(string message) : base(message)
    {
    }

    public static void ThrowIfNull(object o, string message = "Entity not found")
    {
        if (o == null)
        {
            throw new EntityNotFoundException(message);
        }
    }
}
