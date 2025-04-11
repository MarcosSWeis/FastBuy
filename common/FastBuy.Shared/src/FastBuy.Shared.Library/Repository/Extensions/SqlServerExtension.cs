using FastBuy.Shared.Library.Repository.Abstractions;
using FastBuy.Shared.Library.Repository.Implementations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FastBuy.Shared.Library.Repository.Extensions
{
    public static class SqlServerExtension
    {
        public static IServiceCollection AddSqlServerRepository<TEntity, TContext>(this IServiceCollection services)
            where TEntity : class, IBaseEntity
            where TContext : DbContext
        {

            services.TryAddScoped<IRepository<TEntity>>(serviceProvider =>
            {
                var dbContext = serviceProvider.GetRequiredService<DbContext>();
                return new SqlServerDbRepository<TEntity>(dbContext);
            });

            return services;
        }
    }
}
