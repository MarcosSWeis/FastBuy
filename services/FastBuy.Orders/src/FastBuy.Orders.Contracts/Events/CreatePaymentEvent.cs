using MassTransit;

namespace FastBuy.Orders.Contracts.Events
{
    public record CreatePaymentEvent(Guid OrderId,decimal Amount,Guid CustomerId,Guid CorrelationId) :CorrelatedBy<Guid>;
}
