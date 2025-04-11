using FastBuy.Shared.Library.Configurations;
using FastBuy.Shared.Library.Repository.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FastBuy.Shared.Library.Repository.Implementations
{
    public class SqlServerDataBase<TContext> :IDataBase where TContext : DbContext
    {
        public void Configure(IServiceCollection services,IConfiguration configuration)
        {
            var sqlSettings = configuration.GetSection(nameof(SqlServerSettings)).Get<SqlServerSettings>();

            if (sqlSettings is null)
                throw new InvalidOperationException("Error al cargar la configuracion de SQL Server.");

            services.AddDbContext<TContext>(options =>
            {
                options.UseSqlServer(sqlSettings.DefaultConnection);
            });
        }

        public void RegisterRepositories<TEntity>(IServiceCollection services,string? collectionName) where TEntity : class, IBaseEntity
        {
            services.TryAddScoped<IRepository<TEntity>>(serviceProvider =>
            {
                var context = serviceProvider.GetRequiredService<DbContext>();
                return new SqlServerDbRepository<TEntity>(context);
            });
        }

    }
}
