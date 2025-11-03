using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace MedicSoft.Repository.Context
{
    public class MedicSoftDbContextFactory : IDesignTimeDbContextFactory<MedicSoftDbContext>
    {
        public MedicSoftDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MedicSoftDbContext>();
            
            // Read configuration from appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "..", "MedicSoft.Api"))
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? "Host=localhost;Port=5432;Database=medicwarehouse;Username=postgres;Password=postgres";
            
            // Auto-detect database provider
            if (IsPostgreSQL(connectionString))
            {
                optionsBuilder.UseNpgsql(connectionString, options =>
                {
                    options.MigrationsHistoryTable("__EFMigrationsHistory", "public");
                });
            }
            else
            {
                optionsBuilder.UseSqlServer(connectionString);
            }

            return new MedicSoftDbContext(optionsBuilder.Options, configuration);
        }

        private bool IsPostgreSQL(string connectionString)
        {
            return connectionString.Contains("Host=", StringComparison.OrdinalIgnoreCase) ||
                   connectionString.Contains("postgres", StringComparison.OrdinalIgnoreCase) ||
                   connectionString.Contains("Server=", StringComparison.OrdinalIgnoreCase) && 
                   connectionString.Contains("Port=5432", StringComparison.OrdinalIgnoreCase);
        }
    }
}
