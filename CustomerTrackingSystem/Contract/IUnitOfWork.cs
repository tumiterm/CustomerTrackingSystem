using System.Linq.Expressions;

namespace CustomerTrackingSystem.Contract
{
    public interface IUnitOfWork<T> where T : class
    {
        Task<List<T>> OnLoadItemsAsync();
        Task<T> OnLoadItemAsync(Guid Id);
        Task<T> OnItemCreationAsync(T t);
        Task<T> OnModifyItemAsync(T t);
        Task<int> OnRemoveItemAsync(Guid Id);
        Task<int> ItemSaveAsync();
        bool DoesEntityExist<TEntity>(Expression<Func<TEntity, bool>> predicate = null) where TEntity : class;
    }
}
