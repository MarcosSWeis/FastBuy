namespace FastBuy.Payments.Contracts.Events
{
    public record PaymentSucceeded(Guid OrderId,Guid CorrelationId);
}
