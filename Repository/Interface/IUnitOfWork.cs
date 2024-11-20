using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace profescipta.Repository;
    public interface IUnitOfWork
    {
        ICustomerRepository Customer { get; }
        IOrderRepository Order { get; }
         IItemRepository Item { get; }
        Task SaveAsync();
        int Save();
    }

