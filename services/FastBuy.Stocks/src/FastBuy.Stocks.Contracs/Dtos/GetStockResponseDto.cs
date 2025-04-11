namespace FastBuy.Stocks.Contracs.Dtos
{
    public record GetStockResponseDto(
        Guid ProductId,
        int Stock,
        string name,
        string description,
        DateTimeOffset LastUpdated
        );
}
