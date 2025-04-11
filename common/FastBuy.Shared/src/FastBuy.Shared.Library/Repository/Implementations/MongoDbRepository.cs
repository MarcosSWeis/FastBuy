using FastBuy.Shared.Library.Repository.Abstractions;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace FastBuy.Shared.Library.Repository.Implementations
{
    public class MongoDbRepository<T> :IRepository<T> where T : IBaseEntity
    {
        private readonly IMongoCollection<T> dbColecction;
        private readonly FilterDefinitionBuilder<T> filterBuilder = Builders<T>.Filter;

        public MongoDbRepository(IMongoDatabase database,string colectionName)
        {
            ArgumentNullException.ThrowIfNull(database);
            ArgumentException.ThrowIfNullOrEmpty(colectionName);

            dbColecction = database.GetCollection<T>(colectionName);

        }

        public async Task<Guid> CreateAsync(T entity)
        {
            ArgumentNullException.ThrowIfNull(entity);
            await dbColecction.InsertOneAsync(entity);

            return entity.Id;
        }

        public async Task<IReadOnlyCollection<T>> GetAllAsync()
        {
            return await dbColecction.Find(filterBuilder.Empty).ToListAsync();
        }

        public async Task<IReadOnlyCollection<T>> GetAllAsync(Expression<Func<T,bool>> filter)
        {
            ArgumentNullException.ThrowIfNull(filter);

            return await dbColecction.Find(filter).ToListAsync();
        }

        public async Task<T?> GetAsync(Guid id)
        {
            var filter = filterBuilder.Eq(entity => entity.Id,id);

            return await dbColecction.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<T?> GetAsync(Expression<Func<T,bool>> filter)
        {
            ArgumentNullException.ThrowIfNull(filter);

            return await dbColecction.Find(filter).FirstOrDefaultAsync();
        }

        public async Task RemoveAsync(Guid id)
        {
            var filter = filterBuilder.Eq(existingEntity => existingEntity.Id,id);

            var result = await dbColecction.DeleteOneAsync(filter);

            if (result.DeletedCount == 0)
                throw new KeyNotFoundException($"The record with id {id} wasn't found and couldn't be deleted.");
        }

        public async Task UpdateAsync(T entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            var filter = filterBuilder.Eq(existingEntity => existingEntity.Id,entity.Id);

            var result = await dbColecction.ReplaceOneAsync(filter,entity);

            if (result.MatchedCount == 0)
                throw new KeyNotFoundException($"The record with id {entity.Id} wasn't found and couldn't be updated.");

        }
    }
}
