
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace profescipta.Repository;

    public class Repository<T> : IRepository<T> where T : class
    {

        private readonly AppDbContext _db;
        internal DbSet<T> dbSet;

        public Repository(AppDbContext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>();
        }

        public async ValueTask<EntityEntry<T>> Add(T entity)
        {
            return await dbSet.AddAsync(entity);
        }

        public void Delete(T entity)
        {
            dbSet.Remove(entity);
        }

        public async Task<T?> FindSingle(Expression<Func<T, bool>> predicate)
        {
            IQueryable<T> query = dbSet;
            query = query.Where(predicate);
            return await query.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> Find(Expression<Func<T, bool>> predicate)
        {
            IQueryable<T> query = dbSet;
            query = query.Where(predicate);
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            IQueryable<T> query = dbSet;
            return await query.ToListAsync();
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            dbSet.RemoveRange(entities);
        }

        public void UpdateRange(IEnumerable<T> entities)
        {
            dbSet.UpdateRange(entities);
        }

        public void AddRange(IEnumerable<T> entities)
        {
            dbSet.AddRange(entities);
        }

        public void Update(T entity)
        {
            dbSet.Update(entity);
        }

        public IQueryable<T> GetAllQueryable()
        {
            return dbSet.AsQueryable();
        }
    }

