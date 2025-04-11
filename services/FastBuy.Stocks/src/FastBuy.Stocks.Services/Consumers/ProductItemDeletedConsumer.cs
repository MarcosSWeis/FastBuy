using FastBuy.Products.Contracts.Events;
using FastBuy.Shared.Library.Repository.Abstractions;
using FastBuy.Stocks.Entities;
using MassTransit;

namespace FastBuy.Stocks.Services.Consumers
{
    public class ProductItemDeletedConsumer :IConsumer<ProductDeleted>
    {
        private readonly IRepository<ProductItem> repository;

        public ProductItemDeletedConsumer(IRepository<ProductItem> repository)
        {
            this.repository = repository;
        }

        public async Task Consume(ConsumeContext<ProductDeleted> context)
        {
            ProductDeleted message = context.Message;
            var item = await repository.GetAsync(message.Id);
            if (item is null)
                return;

            await repository.RemoveAsync(item.Id);
        }
    }
}
