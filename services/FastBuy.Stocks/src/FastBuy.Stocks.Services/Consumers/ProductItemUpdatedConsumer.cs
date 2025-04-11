using FastBuy.Products.Contracts.Events;
using FastBuy.Shared.Library.Repository.Abstractions;
using FastBuy.Stocks.Entities;
using MassTransit;

namespace FastBuy.Stocks.Services.Consumers
{
    public class ProductItemUpdatedConsumer :IConsumer<ProductUpdated>
    {
        private readonly IRepository<ProductItem> repository;

        public ProductItemUpdatedConsumer(IRepository<ProductItem> repository)
        {
            this.repository = repository;
        }

        public async Task Consume(ConsumeContext<ProductUpdated> context)
        {
            ProductUpdated message = context.Message;
            var item = await repository.GetAsync(message.Id);

            if (item is null)
            {
                item = new ProductItem()
                {
                    Id = message.Id,
                    Name = message.Name,
                    Description = message.Description,
                };

                await repository.CreateAsync(item);

            } else
            {
                item.Name = message.Name;
                item.Description = message.Description;

                await repository.UpdateAsync(item);
            }


        }
    }
}
