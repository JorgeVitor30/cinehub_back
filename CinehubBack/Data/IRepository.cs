using System.Reflection.Metadata;
using CinehubBack.Model;

namespace CinehubBack.Data;

public interface IRepository<T>
    where T : BaseEntity
{
    IQueryable<T> Queryable { get; }
    T? GetById(Guid id);
    Page<U> GetAll<U>(Func<IQueryable<T>, IQueryable<U>> query, Parameter parameter);
    List<U> GetAllList<U>(Func<IQueryable<T>, IQueryable<U>> query);
    T Create(T entity);
    ICollection<T> CreateRange(ICollection<T> entities);
    T Update(T entity);
    ICollection<T> UpdateRange(ICollection<T> entities);
    void Delete(T entity);
    void DeleteById(Guid id);
    U Raw<U>(Func<IQueryable<T>, U> queryable);
    void SaveChanges();
}