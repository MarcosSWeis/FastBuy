using MassTransit;

namespace FastBuy.Orders.Contracts.Events
{
    /// <summary>
    /// Evento de pompensación para una venta emitida por el evento <see cref="StockDecreased"/>
    /// </summary>
    /// <param name="ProductItemId"></param>
    /// <param name="Quantity"></param>
    /// <param name="CorrelationId"></param>
    public record StockIncreased(Guid ProductItemId,int Quantity,Guid CorrelationId) :CorrelatedBy<Guid>;
}
