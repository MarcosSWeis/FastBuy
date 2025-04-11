using FastBuy.Shared.Library.Repository.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FastBuy.Shared.Library.Repository.Implementations
{
    public class SqlServerDbRepository<T> :IRepository<T> where T : class, IBaseEntity
    {
        private readonly DbContext dbContext;
        private readonly DbSet<T> dbSet;

        public SqlServerDbRepository(DbContext dbContext)
        {
            ArgumentNullException.ThrowIfNull(dbContext);
            this.dbContext = dbContext;
            this.dbSet = dbContext.Set<T>();
        }

        public async Task<Guid> CreateAsync(T entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            dbSet.Add(entity);
            await dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<IReadOnlyCollection<T>> GetAllAsync()
        {
            //AsNoTracking agregar a todas las operacion de consulta 
            //por que solo estamos leyendo , si fuera una operacion de esciruta deberia sacarlo por temas de concurrencia
            return await dbSet.AsNoTracking().ToListAsync();
        }

        public async Task<IReadOnlyCollection<T>> GetAllAsync(Expression<Func<T,bool>> filter)
        {
            ArgumentNullException.ThrowIfNull(filter);

            return await dbSet.AsNoTracking().Where(filter).ToListAsync();
        }

        public async Task<T?> GetAsync(Guid id)
        {
            return await dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<T?> GetAsync(Expression<Func<T,bool>> filter)
        {
            ArgumentNullException.ThrowIfNull(filter);

            return await dbSet.AsNoTracking().FirstOrDefaultAsync(filter);
        }

        public async Task RemoveAsync(Guid id)
        {
            var entity = await this.GetAsync(id);
            if (entity is null)
                throw new KeyNotFoundException($"The record with id {id} wasn't found and clouldn't be deleted.");
            dbSet.Remove(entity);
            await dbContext.SaveChangesAsync();

        }

        public async Task UpdateAsync(T entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            dbSet.Update(entity);
            var result = await dbContext.SaveChangesAsync();
            if (result == 0)
                throw new KeyNotFoundException($"The record with id {entity.Id} wasn't found and clouldn't be updated.");


        }
    }
}
