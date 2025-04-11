using FastBuy.Shared.Library.Configurations;
using FastBuy.Shared.Library.Repository.Abstractions;
using FastBuy.Shared.Library.Repository.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace FastBuy.Shared.Library.Repository.Implementations
{
    public class MongoDatabase :IDataBase
    {
        public void Configure(IServiceCollection services,IConfiguration configuration)
        {
            //MongoDB Serealizers
            BsonSerializer.RegisterSerializer(new GuidSerializer(MongoDB.Bson.BsonType.String));
            BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(MongoDB.Bson.BsonType.String));

            var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
            var mongoDbSettings = configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();

            if (mongoDbSettings is null || serviceSettings is null)
                throw new InvalidOperationException($"Error al cargar la configuracion de MongoDB");

            // Registering database
            services.AddSingleton<IMongoDatabase>(serviceProvider =>
            {
                var mongoClient = new MongoClient(mongoDbSettings.ConnectionString);
                return mongoClient.GetDatabase(serviceSettings.ServiceName);
            });
        }

        public void RegisterRepositories<TEntity>(IServiceCollection services,string? collectionName) where TEntity : class, IBaseEntity
        {
            if (string.IsNullOrEmpty(collectionName))
                throw new ArgumentNullException($"El nombre de la colección de proporcionase para MongoDB.",nameof(collectionName));

            services.AddMongoDbRepository<TEntity>(collectionName);

        }
    }
}
