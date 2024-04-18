using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace HotelSearch.Core.Models.Requests.Hotel;

public record CreateHotelRequest(
    [MaxLength(255), NotNull, Required]
    string Name,

    [Range(0, double.MaxValue, MinimumIsExclusive = true)]
    decimal Price,

    [Range(-90, 90, MinimumIsExclusive = false, MaximumIsExclusive = false)]
    double Latitude,

    [Range(-180, 180, MinimumIsExclusive = false, MaximumIsExclusive = false)]
    double Longitude);
