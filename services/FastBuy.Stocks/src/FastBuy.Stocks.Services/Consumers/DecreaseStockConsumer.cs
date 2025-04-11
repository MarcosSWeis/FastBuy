using FastBuy.Orders.Contracts.Events;
using FastBuy.Shared.Library.Repository.Abstractions;
using FastBuy.Stocks.Contracs.Events;
using FastBuy.Stocks.Entities;
using FastBuy.Stocks.Services.Exceptions;
using MassTransit;
namespace FastBuy.Stocks.Services.Consumers
{
    /// <summary>
    /// Evento que se dispara al comprar.
    /// Consumidor que esta suscrito para escuchar evento 'DecreaseStock', es decir el decremento de stock 
    /// </summary>
    public class DecreaseStockConsumer :IConsumer<StockDecreased>
    {
        public readonly IRepository<ProductItem> productRepository;
        public readonly IRepository<StockItem> stockRepository;
        public DecreaseStockConsumer(IRepository<ProductItem> productRepository,IRepository<StockItem> stockRepository)
        {
            this.productRepository = productRepository;
            this.stockRepository = stockRepository;
        }

        public async Task Consume(ConsumeContext<StockDecreased> context)
        {
            var message = context.Message;


            var stockItem = await stockRepository.GetAsync(x => x.ProductId == message.ProductItemId);

            if (stockItem is not null)
            {
                stockItem.Stock -= message.Quantity;
                await stockRepository.UpdateAsync(stockItem);
            } else
            {
                throw new NonExistentProductException(message.ProductItemId);
            }

            await context.Publish(new StockDecreasedCompleted(message.CorrelationId));

        }
    }
}
