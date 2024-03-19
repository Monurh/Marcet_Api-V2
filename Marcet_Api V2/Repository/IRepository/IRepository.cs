using System.Linq.Expressions;

namespace Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task CreateAsync(T entity);
        Task RemoveAsync(T entity);
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null);
        Task<T> GetAsync(Expression<Func<T, bool>> filter = null, bool tracked = true);
        Task UpdateAsync(T entity);
        Task SaveAsync();
        Task DeleteAsync(Expression<Func<T, bool>> filter);
        IQueryable<T> Query();
        Task<T> GetAsync(Guid id);
    }
}
