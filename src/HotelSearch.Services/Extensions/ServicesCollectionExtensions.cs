using HotelSearch.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HotelSearch.BL.Extensions;

public static class ServicesCollectionExtensions
{
    public static void RegisterBl(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IHotelService, HotelService>();
    }
}
