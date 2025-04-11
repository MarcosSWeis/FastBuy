namespace FastBuy.Payments.Services.Consumers
{
    public class CreatePaymentEventConsumer :IConsumer<CreatePaymentEvent>
    {
        public readonly IRepository<Payment> repository;
        private readonly OrderClient orderClient;

        public CreatePaymentEventConsumer(IRepository<Payment> repository,OrderClient orderClient)
        {
            this.repository = repository;
            this.orderClient = orderClient;
        }

        public async Task Consume(ConsumeContext<CreatePaymentEvent> context)
        {


            var message = context.Message;

            var payment = new Payment()
            {
                OrderId = message.OrderId,
                Amount = message.Amount,
                CustomerId = message.CustomerId,
                CreatedAt = DateTimeOffset.UtcNow
            };
            try
            {
                var orderInfClient = await orderClient.GetStatusOrderByCorrelationIdAsync(message.OrderId);

                if (orderInfClient is null || (orderInfClient.CorrelationId != message.CorrelationId || orderInfClient.OrderId != message.OrderId))
                {
                    payment.Status = PaymentStatus.Rejected.ToString();
                    await repository.CreateAsync(payment);
                    await context.Publish(new PaymentFailed(message.OrderId,message.CorrelationId,"Orden no encontrada"));
                }

                bool success = new PaymentProcessor().Procesar(message.Amount);

                // Simulación del pago 
                if (success)
                {
                    payment.Status = PaymentStatus.Completed.ToString();
                    await repository.CreateAsync(payment);
                    await context.Publish(new PaymentSucceeded(message.OrderId,message.CorrelationId));
                } else
                {
                    payment.Status = PaymentStatus.Rejected.ToString();
                    await repository.CreateAsync(payment);
                    await context.Publish(new PaymentFailed(message.OrderId,message.CorrelationId,"Pago rechazado"));
                }


            } catch (Exception ex)
            {
                payment.Status = PaymentStatus.Pending.ToString();
                await repository.CreateAsync(payment);
                await context.Publish(new PaymentFailed(message.OrderId,message.CorrelationId,$"Error inesperado"));
            }
        }
    }
}
