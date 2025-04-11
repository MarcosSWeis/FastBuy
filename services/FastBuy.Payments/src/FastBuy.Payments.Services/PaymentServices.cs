using FastBuy.Payments.Contracts.Dtos;
using FastBuy.Payments.Contracts.Events;
using FastBuy.Payments.Entities;
using FastBuy.Payments.Services.Exceptions;
using FastBuy.Shared.Library.Repository.Abstractions;
using MassTransit;

namespace FastBuy.Payments.Services
{
    public class PaymentServices :IPaymentServices
    {
        private readonly IRepository<Payment> paymenttRepository;
        private readonly IPublishEndpoint publishEndpoint;
        private readonly IRepository<OrderItem> orderRepository;
        public PaymentServices(
            IRepository<OrderItem> orderRepository,
            IRepository<Payment> paymenttRepository,
             IPublishEndpoint publishEndpoint
            )
        {
            this.orderRepository = orderRepository;
            this.paymenttRepository = paymenttRepository;
            this.publishEndpoint = publishEndpoint;
        }

        public async Task<bool> Payment(CreatePaymentDto createPayment)
        {

            var order = await orderRepository.GetAsync(createPayment.OrderId);

            if (order is null)
            {
                throw new UnknowItemExceptions(createPayment.OrderId);
            }

            var payment = new Payment()
            {
                OrderId = createPayment.OrderId,
                Amount = order.Amount,
                CustomerId = order.CustomerId,
                CreatedAt = DateTimeOffset.UtcNow
            };

            PaymentStatus statusPayment = new PaymentProcessor().Procesar(order.Amount);

            bool status = false;

            switch (statusPayment)
            {
                case PaymentStatus.Completed:
                {
                    payment.Status = statusPayment.ToString();
                    await paymenttRepository.CreateAsync(payment);
                    await publishEndpoint.Publish(new PaymentSucceeded(order.Id,order.CorrelationId));
                    status = true;
                    break;
                }

                case PaymentStatus.Rejected:
                {
                    payment.Status = statusPayment.ToString();
                    await paymenttRepository.CreateAsync(payment);
                    await publishEndpoint.Publish(new PaymentFailed(order.Id,order.CorrelationId,$"The payment status is {payment.Status}"));
                    break;
                }
                default:
                {
                    payment.Status = statusPayment.ToString();
                    await paymenttRepository.CreateAsync(payment);
                    await publishEndpoint.Publish(new PaymentFailed(order.Id,order.CorrelationId,$"The payment status is {payment.Status}"));
                    break;
                }
            }

            return status;

        }
    }
}
