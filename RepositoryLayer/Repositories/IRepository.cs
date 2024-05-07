using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace RepositoryLayer.Repositories
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task SaveChangesAsync();
        void Remove(T entity);
        void Update(T entity, T unchanged);
        Task CreateAsync(T entity);
        Task<T?> FindAsync(int id);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> exp);
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);

        List<T> GetList(Expression<Func<T, bool>>? exp = null,
                               Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                               Func<IQueryable<T>, IIncludableQueryable<T, object?>>? include = null,
                               bool enableTracking = true);

        T? Get(Expression<Func<T, bool>> exp,
                      Func<IQueryable<T>, IIncludableQueryable<T, object?>>? include = null,
                      bool enableTracking = true);

        Task<T?> GetAsync(Expression<Func<T, bool>> exp,
                                       Func<IQueryable<T>, IIncludableQueryable<T, object?>>? include = null,
                                       bool enableTracking = true);

        Task<List<T>> GetListAsync(Expression<Func<T, bool>>? exp = null,
                                                Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                                                Func<IQueryable<T>, IIncludableQueryable<T, object?>>? include = null,
                                                bool enableTracking = true);
    }
}
