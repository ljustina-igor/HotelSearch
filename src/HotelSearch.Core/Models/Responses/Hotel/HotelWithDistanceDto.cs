namespace HotelSearch.Core.Models.Responses.Hotel;

public class HotelWithDistanceDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public decimal Price { get; init; }
    public double DistanceInMeters { get; init; }
}
