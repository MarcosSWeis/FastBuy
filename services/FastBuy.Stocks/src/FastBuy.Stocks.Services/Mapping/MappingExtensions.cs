using FastBuy.Stocks.Contracs.Dtos;
using FastBuy.Stocks.Entities;

namespace FastBuy.Stocks.Services.Mapping
{
    internal static class MappingExtensions
    {
        public static GetStockResponseDto MapToGetStockResponseDto(this StockItem stockItem,ProductItem productItem)
        {
            return new GetStockResponseDto(stockItem.ProductId,stockItem.Stock,productItem.Name,productItem.Description,stockItem.LastUpdated);
        }

    }
}
