
namespace profescipta.Repository;

    public class UnitOfWork : IUnitOfWork
    {
        private AppDbContext _db;

        public ICustomerRepository Customer { get; private set; }
        public IOrderRepository Order { get; private set; }
         public IItemRepository Item { get; private set; }


    public UnitOfWork(AppDbContext db)
        {
            _db = db;
            Customer = new CustomerRepository(_db);
            Order = new OrderRepository(_db);
            Item = new ItemRepository(_db);
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }

        public int Save()
        {
            return _db.SaveChanges();
        }
    }

