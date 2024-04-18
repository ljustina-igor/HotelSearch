namespace HotelSearch.Core;

public class HotelSearchConsts
{
    /// <summary>
    /// Spatial reference Id. Used by <see cref="NetTopologySuite.NtsGeometryServices.Instance"> for coordinate factory
    /// </summary>
    public const int Srid = 4326;

    public const string HotelCreatedMessage = "New hotel created.";
    public const string HotelUpdatedMessage = "Hotel updated.";
    public const string HotelDeletedMessage = "hotel deleted.";
}
