using FastBuy.Shared.Library.Repository.Abstractions;

namespace FastBuy.Products.Entities
{
    public class Product :IBaseEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = default!;
        public decimal Price { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
