using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace profescipta.Repository;

    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAll();

        IQueryable<T> GetAllQueryable();

        Task<T?> FindSingle(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> Find(Expression<Func<T, bool>> predicate);
        ValueTask<EntityEntry<T>> Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entities);
        void UpdateRange(IEnumerable<T> entities);
        void AddRange(IEnumerable<T> entities);
    }
