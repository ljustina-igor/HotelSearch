using HotelSearch.Dal.Configurtion;
using HotelSearch.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelSearch.Dal;

public class HotelSearchDbContext : DbContext
{
    public HotelSearchDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HotelConfiguration).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Hotel> Hotels { get; set; }
}
