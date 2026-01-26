using Microsoft.EntityFrameworkCore;

namespace PatientPortal.Application.Interfaces;

/// <summary>
/// Interface for accessing the main MedicSoft database
/// Provides raw query capabilities for appointment booking operations
/// </summary>
public interface IMainDatabaseContext
{
    /// <summary>
    /// Executes a raw SQL query
    /// </summary>
    Task<List<T>> ExecuteQueryAsync<T>(string sql, params object[] parameters) where T : class, new();

    /// <summary>
    /// Executes a raw SQL command (INSERT, UPDATE, DELETE)
    /// </summary>
    Task<int> ExecuteCommandAsync(string sql, params object[] parameters);
}
