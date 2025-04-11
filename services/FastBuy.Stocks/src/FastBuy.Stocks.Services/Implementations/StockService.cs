using FastBuy.Shared.Library.Repository.Abstractions;
using FastBuy.Stocks.Contracs.Dtos;
using FastBuy.Stocks.Entities;
using FastBuy.Stocks.Services.Abstractions;
using FastBuy.Stocks.Services.Mapping;

namespace FastBuy.Stocks.Services.Implementations
{
    public class StockService :IStockService
    {

        private readonly IRepository<StockItem> stockRepository;
        private readonly IRepository<ProductItem> prouctRepository;
        public StockService(
            IRepository<StockItem> stockRepository,
            IRepository<ProductItem> prouctRepository
            )
        {
            this.stockRepository = stockRepository;
            this.prouctRepository = prouctRepository;
        }

        public async Task<bool> DecreaseStock(DecreaseStockRequestDto decreaseStockRequestDto)
        {
            var stockItem = await stockRepository.GetAsync(x => x.ProductId == decreaseStockRequestDto.ProductId);


            if (stockItem is null)
            {
                return false;
            }

            if (stockItem is not null && stockItem.Stock >= decreaseStockRequestDto.Quantity)
            {

                stockItem.Stock -= decreaseStockRequestDto.Quantity;

                await stockRepository.UpdateAsync(stockItem);
                return true;
            }

            return false;

        }

        public async Task<GetStockResponseDto> GetStock(Guid productId)
        {
            var stockItem = await stockRepository.GetAsync(x => x.ProductId == productId);

            if (stockItem is null)
                return null;

            var productItem = await prouctRepository.GetAsync(x => x.Id == stockItem.ProductId);

            return stockItem.MapToGetStockResponseDto(productItem);
        }

        public async Task<bool> SetStock(Guid productId,int stock)
        {
            var stockItem = await stockRepository.GetAsync(x => x.ProductId == productId);

            if (stockItem is null)
            {
                stockItem = new StockItem()
                {
                    Id = Guid.NewGuid(),
                    ProductId = productId,
                    Stock = stock,
                    LastUpdated = DateTimeOffset.UtcNow,
                };

                await stockRepository.CreateAsync(stockItem);
            } else
            {
                stockItem.Stock = stock;
                stockItem.LastUpdated = DateTimeOffset.UtcNow;

                await stockRepository.UpdateAsync(stockItem);
            }

            return true;
        }
    }
}
