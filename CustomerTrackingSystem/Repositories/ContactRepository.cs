using CustomerTrackingSystem.Contract;
using CustomerTrackingSystem.Data;
using CustomerTrackingSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CustomerTrackingSystem.Repositories
{
    public class ContactRepository : IUnitOfWork<Contact>
    {
        private readonly ApplicationDbContext _dbContext;
        public ContactRepository(ApplicationDbContext dbContext)
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
        public async Task<Contact> OnItemCreationAsync(Contact contact)
        {
            try
            {
                await _dbContext.AddAsync(contact);

                return contact;
            }
            catch (Exception)
            {

                throw new Exception("Error: Failed to add address");
            }
        }
        public async Task<Contact> OnLoadItemAsync(Guid ContactId)
        {
            try
            {
                var contact = await _dbContext.Contacts.Where(m => m.ContactId == ContactId).FirstOrDefaultAsync();

                if (contact is null)
                {
                    throw new Exception("Error: Error loading Contact!");
                }

                return contact;

            }
            catch (Exception)
            {

                throw new Exception("Error: Unable to load address");
            }
        }
        public async Task<List<Contact>> OnLoadItemsAsync()
        {
            try
            {
                return await _dbContext.Contacts.ToListAsync();

            }
            catch (Exception)
            {

                throw new Exception("Error: Contacts could not be loaded!");
            }
        }
        public async Task<Contact> OnModifyItemAsync(Contact contact)
        {
            Contact results = new();

            try
            {
                results = await _dbContext.Contacts.FirstOrDefaultAsync(x => x.ContactId == contact.ContactId);

                if (results != null)
                {

                    results.Telephone = contact.Telephone;

                    results.CustomerId = contact.CustomerId;

                    results.ContactPersonName = contact.ContactPersonName;

                    results.ContactPersonEmail = contact.ContactPersonEmail;


                    await _dbContext.SaveChangesAsync();

                }
            }
            catch (Exception)
            {

                throw new Exception("Error: Contact Failed to Modify!");
            }

            return results;
        }
        public async Task<int> OnRemoveItemAsync(Guid ContactId)
        {
            Contact contact = new();

            int record = 0;

            try
            {
                contact = await _dbContext.Contacts.FirstOrDefaultAsync(m => m.ContactId == ContactId);

                if (contact != null)
                {
                    _dbContext.Remove(contact);

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
