using profescipta.Models;

namespace profescipta.Repository;
public interface ICustomerRepository: IRepository<Customer>
{
    // Task<IEnumerable<Customer>> GetCustomers();
    // Task<Customer> GetCustomer(int CustomerId);
}