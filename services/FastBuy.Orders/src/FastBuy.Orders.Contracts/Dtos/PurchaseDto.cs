namespace FastBuy.Orders.Contracts.Dtos
{
    public record PurchaseDto(
        Guid ItemId,
        int Quantity,
        string State,
        string Reason,
        DateTimeOffset Received,
        DateTimeOffset LastUpdated);
}
