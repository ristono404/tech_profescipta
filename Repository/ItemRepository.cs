
using profescipta.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace profescipta.Repository;


    public class ItemRepository : Repository<Item>, IItemRepository
    {
        private AppDbContext _db { get; set; }
        public ItemRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }
    }
