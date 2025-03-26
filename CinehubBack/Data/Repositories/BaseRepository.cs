using CinehubBack.Model;
using Microsoft.EntityFrameworkCore;

namespace CinehubBack.Data.Repositories;

public class BaseRepository<T> : IRepository<T>
    where T : BaseEntity
{
    private readonly DbSet<T> _entities;
    private readonly DatabaseContext _dbContext;
    public IQueryable<T> Queryable => _entities.AsQueryable();

    public BaseRepository(DatabaseContext dbContext)
    {
        _entities = dbContext.Set<T>();
        _dbContext = dbContext;
    }

    public T Create(T entity)
    {
        _entities.Add(entity);
        return entity;
    }

    public ICollection<T> CreateRange(ICollection<T> entities)
    {
        _entities.AddRange(entities);
        return entities;
    }

    public void Delete(T entity)
    {
        _entities.Remove(entity);
    }

    public void DeleteById(Guid id)
    {
        var entity = _entities.FirstOrDefault(e => e.Id == id);
        if (entity != null)
            _entities.Remove(entity);
    }

    public Page<U> GetAll<U>(Func<IQueryable<T>, IQueryable<U>> query, Parameter parameter)
    {
        var queried = query(_entities);
        var count = queried.Count();
        queried = queried.Skip(parameter.Page * parameter.Size).Take(parameter.Size);

        return new Page<U>
        {
            Content = queried.ToList(),
            Total = count,
            CurrentPage = parameter.Page,
            Size = parameter.Size,
        };
    }

    public T? GetById(Guid id)
    {
        return _entities.FirstOrDefault(e => e.Id == id);
    }

    public T Update(T entity)
    {
        _entities.Update(entity);
        return entity;
    }

    public ICollection<T> UpdateRange(ICollection<T> entities)
    {
        _entities.UpdateRange(entities);
        return entities;
    }

    public U Raw<U>(Func<IQueryable<T>, U> queryable)
    {
        return queryable(_entities.AsQueryable());
    }

    public void SaveChanges()
    {
        _dbContext.SaveChanges();
    }
}