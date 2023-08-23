using CustomerTrackingSystem.Contract;
using CustomerTrackingSystem.Data;
using CustomerTrackingSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CustomerTrackingSystem.Repositories
{
    public class AddressRepository : IUnitOfWork<Address>
    {
        private readonly ApplicationDbContext _dbContext;
        public AddressRepository(ApplicationDbContext dbContext)
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
        public async Task<Address> OnItemCreationAsync(Address address)
        {
            try
            {
                await _dbContext.AddAsync(address);

                return address;
            }
            catch (Exception)
            {

                throw new Exception("Error: Failed to add address");
            }
        }
        public async Task<Address> OnLoadItemAsync(Guid AddressId)
        {
            try
            {
                var address = await _dbContext.Addresses.Where(m => m.AddressId == AddressId).FirstOrDefaultAsync();

                if (address is null)
                {
                    throw new Exception("Error!");
                }

                return address;

            }
            catch (Exception)
            {

                throw new Exception("Error: Unable to load address");
            }
        }
        public async Task<List<Address>> OnLoadItemsAsync()
        {
            try
            {
                return await _dbContext.Addresses.ToListAsync();

            }
            catch (Exception)
            {

                throw new Exception("Error: Addresses could not be loaded!");
            }
        }
        public async Task<Address> OnModifyItemAsync(Address address)
        {
            Address results = new();

            try
            {
                results = await _dbContext.Addresses.FirstOrDefaultAsync(x => x.AddressId == address.AddressId);

                if (results != null)
                {

                    results.CustomerId = address.CustomerId;

                    results.StreetComplex = address.StreetComplex;

                    results.City = address.City;

                    results.Surburb = address.Surburb;
                    
                    results.PostalCode = address.PostalCode;

                    await _dbContext.SaveChangesAsync();

                }
            }
            catch (Exception)
            {

                throw new Exception("Error: Address Failed to Modify!");
            }

            return results;
        }
        public async Task<int> OnRemoveItemAsync(Guid AddressId)
        {
            Address address = new();

            int record = 0;

            try
            {
                address = await _dbContext.Addresses.FirstOrDefaultAsync(m => m.AddressId == AddressId);

                if (address != null)
                {
                    _dbContext.Remove(address);

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
