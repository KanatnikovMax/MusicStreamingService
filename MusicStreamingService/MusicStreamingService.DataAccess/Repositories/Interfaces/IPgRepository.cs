using System.Linq.Expressions;

namespace MusicStreamingService.DataAccess.Repositories.Interfaces;

public interface IPgRepository<T>
{
    Task<IEnumerable<T>> FindAllAsync();
    
    Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> predicate);
    
    Task<T?> FindByIdAsync(Guid id);
    
    void Delete(T entity);
    
    T Update(T entity);
}