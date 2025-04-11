using FastBuy.Shared.Library.Repository.Abstractions;
using FastBuy.Shared.Library.Repository.Implementations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver;

namespace FastBuy.Shared.Library.Repository.Extensions
{
    public static class MongoDbExtensions
    {
        public static IServiceCollection AddMongoDbRepository<T>(this IServiceCollection services,string collectionName) where T : IBaseEntity
        {
            services.TryAddSingleton<IRepository<T>>(serviceProvider =>
            {
                var database = serviceProvider.GetRequiredService<IMongoDatabase>();
                return new MongoDbRepository<T>(database,collectionName);
            });
            return services;
        }
    }
}
