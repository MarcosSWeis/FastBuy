namespace FastBuy.Payments.Contracts.Events
{
    public record PaymentFailed(Guid OrderId,Guid CorrelationId,string Reason);
}
