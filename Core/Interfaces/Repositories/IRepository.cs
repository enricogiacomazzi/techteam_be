using Domain.Entities;

namespace Core.Interfaces.Repositories;

public interface IRepository<TEntity, TKey> where TEntity : Entity<TKey>
{
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<TEntity?> GetByIdAsync(TKey id);
    Task CreateAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(TKey id);
}