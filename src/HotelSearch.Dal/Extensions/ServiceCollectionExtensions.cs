using HotelSearch.Core.Repositories;
using HotelSearch.Core.UnitOfWork;
using HotelSearch.Dal.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HotelSearch.Dal.Extensions;

public static class ServiceCollectionExtensions
{
    public static void RegisterDal(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddDbContext<HotelSearchDbContext>(db =>
            db.UseSqlServer(configuration.GetConnectionString("connectionString"), b => b.UseNetTopologySuite()));

        RegisterRepositories(serviceCollection);
        serviceCollection.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
    }

    private static void RegisterRepositories(IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IHotelRepository, HotelRepository>();
    }
}
