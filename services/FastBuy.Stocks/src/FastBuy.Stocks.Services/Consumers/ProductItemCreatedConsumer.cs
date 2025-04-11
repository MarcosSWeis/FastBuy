using FastBuy.Products.Contracts.Events;
using FastBuy.Shared.Library.Repository.Abstractions;
using FastBuy.Stocks.Entities;
using MassTransit;

namespace FastBuy.Stocks.Services.Consumers
{
    public class ProductItemCreatedConsumer :IConsumer<ProductCreated>
    {
        private readonly IRepository<ProductItem> repository;

        public ProductItemCreatedConsumer(IRepository<ProductItem> repository)
        {
            this.repository = repository;
        }
        public async Task Consume(ConsumeContext<ProductCreated> context)
        {
            ProductCreated message = context.Message;

            var item = await repository.GetAsync(message.Id);

            if (item is not null)
                return;

            item = new ProductItem()
            {
                Id = message.Id,
                Name = message.Name,
                Description = message.Description,
            };

            await repository.CreateAsync(item);

        }
    }
}
