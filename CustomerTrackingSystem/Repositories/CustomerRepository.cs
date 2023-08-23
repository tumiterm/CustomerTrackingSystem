
using CustomerTrackingSystem.Contract;
using CustomerTrackingSystem.Data;
using CustomerTrackingSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CustomerTrackingSystem.Repositories
{
    public class CustomerRepository : IUnitOfWork<Customer>
    {
        private readonly ApplicationDbContext _dbContext;
        public CustomerRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public bool DoesEntityExist<TEntity>(Expression<Func<TEntity, bool>> predicate = null) where TEntity : class
        {
            IQueryable<TEntity> data = _dbContext.Set<TEntity>();

            return data.Any(predicate);
        }
        public async Task<int> ItemSaveAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
        public async Task<Customer> OnItemCreationAsync(Customer customer)
        {
            try
            {
                await _dbContext.AddAsync(customer);

                return customer;
            }
            catch (Exception)
            {

                throw new Exception("Error: Failed to add customer");
            }
        }
        public async Task<Customer> OnLoadItemAsync(Guid CustomerId)
        {
            try
            {
                var customer = await _dbContext.Customers.Where(m => m.CustomerId == CustomerId).Include(m => m.Address).Include(m => m.ContactPerson).FirstOrDefaultAsync();

                if (customer is null)
                {
                    throw new Exception("Error!");
                }

                return customer;

            }
            catch (Exception)
            {

                throw new Exception("Error: Unable to load customer");
            }
        }
        public async Task<List<Customer>> OnLoadItemsAsync()
        {
            try
            {
                return await _dbContext.Customers.Include(m => m.Address)
                                                 .Include(m => m.ContactPerson)
                                                 .ToListAsync();

            }
            catch (Exception)
            {

                throw new Exception("Error: Customer could not be loaded!");
            }
        }
        public async Task<Customer> OnModifyItemAsync(Customer customer)
        {
            Customer results = new();

            try
            {
                results = await _dbContext.Customers.Include(m => m.Address)
                                                    .Include(m => m.ContactPerson)
                                                    .FirstOrDefaultAsync(x => x.CustomerId == customer.CustomerId);

                if (results != null)
                {

                    results.Address = customer.Address;

                    results.CustomerName = customer.CustomerName;

                    results.VATNumber = customer.VATNumber;

                    results.ContactPerson = customer.ContactPerson;

                    results.ContactPerson = customer.ContactPerson;
                }

                _dbContext.Entry(results).CurrentValues.SetValues(customer);

                await _dbContext.SaveChangesAsync();

            }
            catch (Exception)
            {

                throw new Exception("Error: Customer Failed to Modify!");
            }

            return results;
        }
        public async Task<int> OnRemoveItemAsync(Guid CustomerId)
        {
            Customer customer = new();

            int record = 0;

            try
            {
                customer = await _dbContext.Customers.FirstOrDefaultAsync(m => m.CustomerId == CustomerId);

                if (customer != null)
                {
                    _dbContext.Remove(customer);

                    record = await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception)
            {

                throw new Exception("Error: Delete Failed");
            }

            return record;
        }
    }
}


