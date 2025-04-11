using MassTransit;

namespace FastBuy.Orders.Contracts.Events
{
    public record OrderItemCreated(Guid OrderId,decimal Amount,Guid CustomerId,Guid CorrelationId) :CorrelatedBy<Guid>;
}
