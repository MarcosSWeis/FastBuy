using FastBuy.Orders.Contracts.Events;
using FastBuy.Payments.Entities;
using FastBuy.Shared.Library.Repository.Abstractions;
using MassTransit;

namespace FastBuy.Payments.Services.Consumers
{
    public class OrderItemCreatedConsumer :IConsumer<OrderItemCreated>
    {
        public readonly IRepository<OrderItem> repository;
        public OrderItemCreatedConsumer(IRepository<OrderItem> repository)
        {
            this.repository = repository;
        }

        public async Task Consume(ConsumeContext<OrderItemCreated> context)
        {
            var message = context.Message;
            var orderItem = new OrderItem()
            {
                Id = message.OrderId,
                Amount = message.Amount,
                CustomerId = message.CustomerId,
                CorrelationId = message.CorrelationId
            };

            await repository.CreateAsync(orderItem);

        }
    }
}
