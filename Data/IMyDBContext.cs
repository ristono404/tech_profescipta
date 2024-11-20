using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
    public interface IMyDBContext
    {
        int SaveChanges();
        DbSet<T> Set<T>() where T : class;
        EntityEntry Entry(object entity);
        //DbEntityEntry Entry(object o);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        //Task<int> SaveChangesAsync();
        void Dispose();
    }

