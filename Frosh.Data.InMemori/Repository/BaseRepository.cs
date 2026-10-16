
using System.Collections.Concurrent;
using Frosh.Core.Common;

namespace Frosh.Data.InMemori.Repository;

public class BaseRepository<TEntity, TId> : IRepository<TEntity, TId>
    where TEntity : Entity<TId>
    where TId : notnull
{
    private readonly ConcurrentDictionary<TId, TEntity> _items = new();

    public IEnumerable<TEntity> GetAll()
    {
        return _items.Values.ToList();
    }

    public TEntity? GetById(TId id)
    {
        return _items.TryGetValue(id, out var entity)
            ? entity
            : null;
    }

    public void Add(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        if (!_items.TryAdd(entity.Id, entity))
        {
            throw new InvalidOperationException(
                $"Entity with id '{entity.Id}' already exists.");
        }
    }

    public bool Update(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return _items.TryGetValue(entity.Id, out var existing)
            && _items.TryUpdate(entity.Id, entity, existing);
    }

    public bool Delete(TId id)
    {
        return _items.TryRemove(id, out _);
    }
}