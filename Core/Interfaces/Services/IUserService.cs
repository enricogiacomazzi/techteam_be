using Domain.Entities;

namespace Core.Interfaces.Services;

public interface IUserService
{
    Task<IEnumerable<User?>> GetAllUserAsync();
    Task<User?> GetUserByIdAsync(Guid id);
    Task<bool> CheckUserExistAsync(Guid id);
    Task<User> CreateUserAsync(User user);
    Task UpdateUserAsync(User user);
    Task DeleteUser(Guid id);
}