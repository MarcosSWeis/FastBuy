using MassTransit;

namespace FastBuy.Orders.Contracts.Events
{
    /// <summary>
    /// Evento para decrementar del stock al momento de una venta.
    /// </summary>
    /// <param name="ProductItemId"></param>
    /// <param name="Quantity"></param>
    /// <param name="CorrelationId"></param>
    public record StockDecreased(Guid ProductItemId,int Quantity,Guid CorrelationId) :CorrelatedBy<Guid>;
}
