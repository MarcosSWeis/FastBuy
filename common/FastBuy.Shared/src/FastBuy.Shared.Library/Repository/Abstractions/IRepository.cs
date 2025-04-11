using System.Linq.Expressions;

namespace FastBuy.Shared.Library.Repository.Abstractions
{
    public interface IRepository<T> where T : IBaseEntity
    {
        Task<IReadOnlyCollection<T>> GetAllAsync();

        Task<IReadOnlyCollection<T>> GetAllAsync(Expression<Func<T,bool>> filter);

        Task<T?> GetAsync(Guid id);
        Task<T?> GetAsync(Expression<Func<T,bool>> filter);
        Task<Guid> CreateAsync(T entity);
        Task UpdateAsync(T entity);
        Task RemoveAsync(Guid id);


    }
}
