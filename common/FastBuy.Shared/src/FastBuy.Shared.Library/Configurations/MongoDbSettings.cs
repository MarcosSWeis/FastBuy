namespace FastBuy.Shared.Library.Configurations
{
    public class MongoDbSettings
    {
        public string Host { get; init; } = string.Empty;
        public string Port { get; init; } = string.Empty;
        public string ConnectionString => $"mongodb://{Host}:{Port}";
    }
}
