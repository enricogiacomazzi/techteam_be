using Domain.Entities;

namespace Core.Interfaces.Repositories;

public interface IUserRepository : IRepository<User, Guid> { }