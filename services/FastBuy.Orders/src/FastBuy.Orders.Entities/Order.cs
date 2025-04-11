using FastBuy.Shared.Library.Repository.Abstractions;

namespace FastBuy.Orders.Entities
{
    public class Order :IBaseEntity
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; }
        public Guid ProductId { get; set; }
        public decimal Total { get; set; }
        public Guid CustomerId { get; set; }
        public Guid CorrelationId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
