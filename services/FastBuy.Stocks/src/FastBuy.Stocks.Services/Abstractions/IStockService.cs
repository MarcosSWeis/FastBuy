using FastBuy.Stocks.Contracs.Dtos;

namespace FastBuy.Stocks.Services.Abstractions
{
    public interface IStockService
    {
        Task<GetStockResponseDto> GetStock(Guid productId);

        Task<bool> SetStock(Guid productId,int stock);

        Task<bool> DecreaseStock(DecreaseStockRequestDto decreaseStockRequestDto);
    }
}
