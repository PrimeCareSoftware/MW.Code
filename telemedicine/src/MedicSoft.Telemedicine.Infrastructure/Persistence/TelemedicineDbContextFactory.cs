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
        
        // Use a default PostgreSQL connection string for migrations
        // This will be overridden at runtime by the actual connection string
        optionsBuilder.UseNpgsql("Host=localhost;Database=medicsoft;Username=postgres;Password=postgres");
        
        return new TelemedicineDbContext(optionsBuilder.Options);
    }
}
