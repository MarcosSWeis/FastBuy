namespace FastBuy.Orders.Contracts
{
    /// <summary>
    /// Mensaje que viajará entre los microservicios cuando se realice la compra.
    /// </summary>
    /// <param name="CorrelationId"></param>
    public record GetPurchaseStateMessage(Guid CorrelationId);

    /// <summary>
    /// Mensaje que viajará entre los microservicios cuando se tenga que decrementar el stock.
    /// </summary>
    /// <param name="CorrelationId"></param>
    public record StockDecreasedMessage(Guid CorrelationId);

    /// <summary>
    /// Mensaje que viajará entre los microservicios cuando tenga que aumentar el stock. 
    /// </summary>
    /// <param name="ProductItemId"></param>
    /// <param name="Quantity"></param>
    /// <param name="CorrelationId"></param>
    public record GrantItemsMessage(Guid ProductItemId,int Quantity,Guid CorrelationId);
}
