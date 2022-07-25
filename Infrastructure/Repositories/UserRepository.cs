
using Core.Interfaces.Repositories;
using Domain.Entities;

namespace Infrastructure.Repositories;

public class UserRepository: IUserRepository
{
    private readonly List<User> _users;

    public UserRepository(MockData mock)
    {
        _users = mock.Users;
    }
    
    public Task<IEnumerable<User>> GetAllAsync() => Task.FromResult(_users.AsEnumerable());

    public Task<User?> GetByIdAsync(Guid id) => Task.FromResult(_users.SingleOrDefault(u => u.Id == id));

    public Task CreateAsync(User entity)
    {
        _users.Add(entity);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(User entity)
    {
        var index = _users.FindIndex(u => u.Id == entity.Id);
        _users[index] = entity;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id)
    {
        var index = _users.FindIndex(u => u.Id == id);
        _users.RemoveAt(index);
        return Task.CompletedTask;
    }


}