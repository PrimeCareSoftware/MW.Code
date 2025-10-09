using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MedicSoft.Repository.Context
{
    public class MedicSoftDbContextFactory : IDesignTimeDbContextFactory<MedicSoftDbContext>
    {
        public MedicSoftDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MedicSoftDbContext>();
            
            // Use a connection string for design-time operations
            // This is just for migrations - actual connection string comes from configuration at runtime
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=MedicSoftDb;Trusted_Connection=True;MultipleActiveResultSets=true");

            return new MedicSoftDbContext(optionsBuilder.Options);
        }
    }
}
