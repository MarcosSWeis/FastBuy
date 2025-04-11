using FastBuy.Shared.Library.Repository.Abstractions;

namespace FastBuy.Payments.Entities
{
    public class OrderItem :IBaseEntity
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public Guid CustomerId { get; set; }
        public Guid CorrelationId { get; set; }
    }
}
