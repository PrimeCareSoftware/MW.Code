using Microsoft.EntityFrameworkCore;
using PatientPortal.Application.Interfaces;
using PatientPortal.Infrastructure.Data;

namespace PatientPortal.Infrastructure.Services;

/// <summary>
/// Implementation of IMainDatabaseContext that provides access to the main MedicSoft database
/// </summary>
public class MainDatabaseContext : IMainDatabaseContext
{
    private readonly PatientPortalDbContext _context;

    public MainDatabaseContext(PatientPortalDbContext context)
    {
        _context = context;
    }

    public async Task<List<T>> ExecuteQueryAsync<T>(string sql, params object[] parameters) where T : class, new()
    {
        var query = _context.Database.SqlQueryRaw<T>(sql, parameters);
        return await query.ToListAsync();
    }

    public async Task<int> ExecuteCommandAsync(string sql, params object[] parameters)
    {
        return await _context.Database.ExecuteSqlRawAsync(sql, parameters);
    }
}
