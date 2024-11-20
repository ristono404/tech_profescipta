
using profescipta.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace profescipta.Repository;


    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        private AppDbContext _db { get; set; }
        public OrderRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }
    }
