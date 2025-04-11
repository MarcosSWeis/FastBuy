
using FastBuy.Orders.Contracts.Events;
using FastBuy.Shared.Library.Repository.Abstractions;
using FastBuy.Stocks.Contracs.Events;
using FastBuy.Stocks.Entities;
using MassTransit;

namespace FastBuy.Stocks.Services.Consumers
{
    /// <summary>
    /// Operacion de compensación.
    /// Aumenta la cantidad de stock que fue decrementado en en el evento <see cref="DecreaseStockConsumer"/>
    /// </summary>
    public class IncreaseStockConsumer :IConsumer<StockIncreased>
    {
        public readonly IRepository<ProductItem> productRepository;
        public readonly IRepository<StockItem> stockRepository;
        public IncreaseStockConsumer(IRepository<ProductItem> productRepository,IRepository<StockItem> stockRepository)
        {
            this.productRepository = productRepository;
            this.stockRepository = stockRepository;
        }

        public async Task Consume(ConsumeContext<StockIncreased> context)
        {
            var message = context.Message;


            var stockItem = await stockRepository.GetAsync(x => x.ProductId == message.ProductItemId);


            if (stockItem is not null)
            {
                stockItem = new StockItem()
                {
                    ProductId = message.ProductItemId,
                    Stock = message.Quantity,
                    LastUpdated = DateTimeOffset.UtcNow,
                };
                await stockRepository.CreateAsync(stockItem);
            } else
            {
                stockItem.Stock += message.Quantity;
                await stockRepository.UpdateAsync(stockItem);
            }

            await context.Publish(new StockIncreasedCompleted(message.CorrelationId));
        }
    }
}
