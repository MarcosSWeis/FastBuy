using FastBuy.Orders.Api.StateMachines;
using FastBuy.Orders.Contracts;
using FastBuy.Orders.Contracts.Dtos;
using FastBuy.Orders.Contracts.Events;
using FastBuy.Orders.Entities;
using FastBuy.Orders.Services.Exceptions;
using FastBuy.Shared.Library.Repository.Abstractions;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace FastBuy.Orders.Api.Controllers
{
    [ApiController]
    [Route("orders")]
    public class OrdersController :ControllerBase
    {
        private readonly IPublishEndpoint publishEndpoint;
        private readonly IRequestClient<GetPurchaseStateMessage> purchaseClient;
        private readonly IRepository<Order> orderRepository;
        private readonly IRepository<ProductItem> productRepository;

        public OrdersController(
            IPublishEndpoint publishEndpoint,
            IRequestClient<GetPurchaseStateMessage> purchaseClient,
            IRepository<Order> orderRepository,
            IRepository<ProductItem> productRepository)
        {
            this.publishEndpoint = publishEndpoint;
            this.purchaseClient = purchaseClient;
            this.orderRepository = orderRepository;
            this.productRepository = productRepository;
        }

        [HttpGet("status/{CorrelationId}")]
        public async Task<ActionResult<PurchaseDto>> GetStatusAsync(Guid CorrelationId)
        {
            var response = await purchaseClient.GetResponse<OrderState>(
                new GetPurchaseStateMessage(CorrelationId)
            );

            var purchaseState = response.Message;

            var purchase = new PurchaseDto(
                purchaseState.ItemId,
                purchaseState.Quantity,
                purchaseState.CurrentState,
                purchaseState.ErrorMessage,
                purchaseState.Received,
                purchaseState.LastUpdated

            );
            return Ok(purchase);
        }
        [HttpGet("{OrderId}")]
        public async Task<ActionResult<PurchaseDto>> GetOrderByIdAsync(Guid OrderId)
        {
            var order = await orderRepository.GetAsync(x => x.Id == OrderId);

            if (order is null)
            {
                return NotFound();
            }

            return Ok(new OrderResponseDto(order.Id,order.CorrelationId));
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(SubmitPurchaseDto purchase)
        {
            var productItem = await productRepository.GetAsync(x => x.Id == purchase.ItemId.Value);

            if (productItem is null)
                throw new UnknowItemExceptions(purchase.ItemId.Value);

            //creo la orden con estado pendiente siempre 
            var order = new Order()
            {
                Quantity = purchase.Quantity,
                ProductId = purchase.ItemId.Value,
                Total = purchase.Quantity * productItem.Price,
                CorrelationId = purchase.CorrelationId
                //estado piendiente
            };

            Guid orderId = await orderRepository.CreateAsync(order);

            var message = new StockDecreased(
                purchase.ItemId.Value,
                purchase.Quantity,
                purchase.CorrelationId
            );

            var messageOrderCreated = new OrderItemCreated(orderId,order.Total,order.CustomerId,order.CorrelationId);

            await publishEndpoint.Publish(message);
            await publishEndpoint.Publish(messageOrderCreated);


            return AcceptedAtAction(
                nameof(GetStatusAsync),
                new { purchase.CorrelationId },
                new { purchase.CorrelationId }
            );
        }

    }
}
