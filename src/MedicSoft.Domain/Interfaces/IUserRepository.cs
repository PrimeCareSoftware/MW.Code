namespace MedicSoft.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<object?> GetByUsernameAsync(string username);
    }
}
