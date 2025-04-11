using FastBuy.Orders.Entities;
using FastBuy.Products.Contracts.Events;
using FastBuy.Shared.Library.Repository.Abstractions;
using MassTransit;

namespace FastBuy.Orders.Services.Consumers
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
                    Price = message.Price,
                };

                await repository.CreateAsync(item);

            } else
            {
                item.Name = message.Name;
                item.Description = message.Description;
                item.Price = message.Price;

                await repository.UpdateAsync(item);
            }


        }
    }
}
