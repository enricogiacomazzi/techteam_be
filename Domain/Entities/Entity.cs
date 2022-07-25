namespace Domain.Entities;

public abstract class Entity<T>
{
    public T Id { get; set; }
}