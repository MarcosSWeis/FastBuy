
using FastBuy.Shared.Library.Repository.Abstractions;
using Microsoft.EntityFrameworkCore;


namespace FastBuy.Shared.Library.Repository.Implementations
{
    public class DatabaseFactory
    {
        /// <summary>
        /// Para bases de datos sin DBContext
        /// </summary>
        /// <returns></returns>
        public static IDataBase CreateDataBase(string provider)
        {
            var normalizedProvider = provider.ToLower().Trim();

            return normalizedProvider switch
            {
                "mongodb" => new MongoDatabase(),
                _ => throw new NotImplementedException($"Data base provider {provider} not implemented.")
            };
        }
        /// <summary>
        /// Para bases de datos que necesiten un contexto
        /// </summary>
        /// <returns></returns>
        public static IDataBase CreateDataBase<TContext>(string provider) where TContext : DbContext
        {
            var normalizedProvider = provider.ToLower().Trim();

            return normalizedProvider switch
            {
                "sqlserver" => new SqlServerDataBase<TContext>(),
                _ => throw new NotImplementedException($"Data base provider {provider} not implemented.")
            };
        }
    }
}
