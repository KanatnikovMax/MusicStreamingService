using System.Linq.Expressions;
using MusicStreamingService.DataAccess.Postgres.Entities;

namespace MusicStreamingService.DataAccess.Postgres.Repositories.Interfaces;

public interface IPgRepository<T>
{
    Task<T?> SaveAsync(T entity);
    
    
    Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> predicate);
    
    Task<T?> FindByIdAsync(Guid id);
    
    void Delete(T entity);
    
    T Update(T entity);
}