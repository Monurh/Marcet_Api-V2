using Marcet_Api_V2.Models;
using Marcet_Api_V2.Repository;
using Microsoft.EntityFrameworkCore;
using Repository.IRepository;
using System.Linq.Expressions;

namespace Marcet_Api.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly MarcetDbContext _context;


        private readonly DbSet<T> _dbSet;
        public Repository(MarcetDbContext context, ILogger<Repository<T>> logger)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task CreateAsync(T entity)
        {
            try
            {
                await _dbSet.AddAsync(entity);
                await SaveAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task RemoveAsync(T entity)
        {
            try
            {
                _dbSet.Remove(entity);
                await SaveAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> filter = null, bool tracked = true)
        {
            try
            {
                IQueryable<T> query = _dbSet;

                if (!tracked)
                {
                    query = query.AsNoTracking();
                }

                if (filter != null)
                {
                    query = query.Where(filter);
                }

                return await query.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null)
        {
            try
            {
                IQueryable<T> query = _dbSet;

                if (filter != null)
                {
                    query = query.Where(filter);
                }

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task UpdateAsync(T entity)
        {
            try
            {
                _dbSet.Update(entity);
                await SaveAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task SaveAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task DeleteAsync(Expression<Func<T, bool>> filter)
        {
            try
            {
                var entitiesToDelete = _dbSet.Where(filter);
                _dbSet.RemoveRange(entitiesToDelete);
                await SaveAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Set<Product>().ToListAsync();
        }

        public IQueryable<T> Query()
        {
            return _dbSet;
        }

        public async Task<T> GetAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }
    }
}
