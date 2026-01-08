using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MedicSoft.Telemedicine.Infrastructure.Persistence;

/// <summary>
/// Design-time factory for creating TelemedicineDbContext during migrations
/// </summary>
public class TelemedicineDbContextFactory : IDesignTimeDbContextFactory<TelemedicineDbContext>
{
    public TelemedicineDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TelemedicineDbContext>();
        
        // DEVELOPMENT ONLY: Default PostgreSQL connection string for migrations
        // In production, connection strings are provided via configuration
        // This will be overridden at runtime by the actual connection string from appsettings
        optionsBuilder.UseNpgsql("Host=localhost;Database=medicsoft;Username=postgres;Password=postgres");
        
        return new TelemedicineDbContext(optionsBuilder.Options);
    }
}
