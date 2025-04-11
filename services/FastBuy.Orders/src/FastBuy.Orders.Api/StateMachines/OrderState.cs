using MassTransit;

namespace FastBuy.Orders.Api.StateMachines
{
    public class OrderState :SagaStateMachineInstance, ISagaVersion
    {
        public Guid CorrelationId { get; set; }
        public int Version { get; set; }
        public Guid ProductId { get; set; }
        public string CurrentState { get; set; }
        public Guid ItemId { get; set; }
        public int Quantity { get; set; }
        public DateTimeOffset Received { get; set; }
        public DateTimeOffset LastUpdated { get; set; }
        public string? ErrorMessage { get; set; }
        /// <summary>
        /// Me permite saber si un mensaje va viejando entre microservicios esta o no compensado.
        /// </summary>
        public bool Compensated { get; set; }

    }
}
