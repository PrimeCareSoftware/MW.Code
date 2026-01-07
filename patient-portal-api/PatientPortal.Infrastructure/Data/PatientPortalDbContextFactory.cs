using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace PatientPortal.Infrastructure.Data;

/// <summary>
/// Factory for creating PatientPortalDbContext at design time for migrations
/// </summary>
public class PatientPortalDbContextFactory : IDesignTimeDbContextFactory<PatientPortalDbContext>
{
    public PatientPortalDbContext CreateDbContext(string[] args)
    {
        var configuration = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../PatientPortal.Api"))
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<PatientPortalDbContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        optionsBuilder.UseNpgsql(connectionString);

        return new PatientPortalDbContext(optionsBuilder.Options);
    }
}
