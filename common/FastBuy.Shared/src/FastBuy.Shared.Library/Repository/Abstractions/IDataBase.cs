using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FastBuy.Shared.Library.Repository.Abstractions
{
    public interface IDataBase
    {
        void Configure(IServiceCollection services,IConfiguration configuration);

        void RegisterRepositories<TEntity>(IServiceCollection services,string? collectionName = null) where TEntity : class, IBaseEntity;
    }
}
