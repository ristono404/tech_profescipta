using Microsoft.EntityFrameworkCore;
using profescipta.Models;

namespace profescipta.Repository;

public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
       private AppDbContext _db { get; set; }
        public CustomerRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }
   
}