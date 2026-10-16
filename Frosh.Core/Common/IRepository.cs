using System.Collections.Generic;
using Frosh.Core.Entities;

namespace Frosh.Core.Common;

public interface IRepository<TEntity, TId>
    where TEntity : Entity<TId>
    where TId : notnull
{
    IEnumerable<TEntity> GetAll();
    TEntity? GetById(TId id);

    void Add(TEntity entity);
    bool Update(TEntity entity);
    bool Delete(TId id);
}
