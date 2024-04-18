using NetTopologySuite.Geometries;

namespace HotelSearch.Entities;

public class Hotel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public Point Coordinates { get; set; }
}
