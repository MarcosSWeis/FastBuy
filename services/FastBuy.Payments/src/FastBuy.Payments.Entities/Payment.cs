using FastBuy.Shared.Library.Repository.Abstractions;

namespace FastBuy.Payments.Entities
{
    public class Payment :IBaseEntity
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }

    public enum PaymentStatus
    {
        Pending = 1,
        Completed = 2,
        Rejected = 3
    }
}
