using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Domain.Entities;

namespace Core.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public Task<IEnumerable<User>> GetAllUserAsync() => _userRepository.GetAllAsync();
    public Task<User?> GetUserByIdAsync(Guid id) => _userRepository.GetByIdAsync(id);
    public async Task<bool> CheckUserExistAsync(Guid id)
    {
        var user = await GetUserByIdAsync(id);
        return user is not null;
    }

    public async Task<User> CreateUserAsync(User user)
    {
        user.Id = Guid.NewGuid();
        await _userRepository.CreateAsync(user);
        var created = await _userRepository.GetByIdAsync(user.Id);
        if (created is null)
        {
            throw new Exception("Error creating user");
        }

        return created;
    }

    public async Task UpdateUserAsync(User user)
    {
        await _userRepository.UpdateAsync(user);
    }

    public async Task DeleteUser(Guid id)
    {
        await _userRepository.DeleteAsync(id);
    }
}