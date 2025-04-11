using FastBuy.Orders.Contracts;
using FastBuy.Orders.Contracts.Events;
using FastBuy.Payments.Contracts.Events;
using FastBuy.Stocks.Contracs.Events;
using MassTransit;

namespace FastBuy.Orders.Api.StateMachines;

public class OrderStateMachine :MassTransitStateMachine<OrderState>
{
    //Estados
    public State Accepted { get; }
    public State StockGranted { get; }
    public State Completed { get; }
    public State Faulted { get; }

    //Eventos
    public Event<GrantItemsMessage> PurchaseRequested { get; }
    public Event<GetPurchaseStateMessage> GetPurchaseState { get; }
    public Event<StockDecreased> StockDecreasedEvent { get; }
    public Event<StockIncreased> StockIncreasedEvent { get; }
    public Event<StockIncreasedCompleted> StockIncreasedCompletedEvent { get; }
    public Event<StockDecreasedCompleted> StockDecreasedCompletedEvent { get; }

    public Event<Fault<GrantItemsMessage>> GrantItemsFaulted { get; }
    public Event<PaymentSucceeded> PaymentSucceededEvent { get; }
    public Event<PaymentFailed> PaymentFailedEvent { get; }

    public OrderStateMachine()
    {
        InstanceState(state => state.CurrentState);
        ConfigureEvents();
        ConfigureInitialState();
        ConfigureAny();
        ConfigureAccepted();
        ConfigureFaulted();
        ConfigureCompleted();
        ConfigureStockGranted();
    }

    private void ConfigureEvents()
    {
        Event(() => PurchaseRequested);
        Event(() => GetPurchaseState);
        Event(() => StockDecreasedEvent,x => x.CorrelateById(m => m.Message.CorrelationId));
        Event(() => StockIncreasedEvent,x => x.CorrelateById(m => m.Message.CorrelationId));
        Event(() => GrantItemsFaulted,x => x.CorrelateById(m => m.Message.Message.CorrelationId));
        Event(() => PaymentSucceededEvent,x => x.CorrelateById(m => m.Message.CorrelationId));
        Event(() => PaymentFailedEvent,x => x.CorrelateById(m => m.Message.CorrelationId));
        Event(() => StockIncreasedCompletedEvent,x => x.CorrelateById(m => m.Message.CorrelationId));
        Event(() => StockDecreasedCompletedEvent,x => x.CorrelateById(m => m.Message.CorrelationId));
    }

    private void ConfigureInitialState()
    {
        Initially(
            When(PurchaseRequested)
                .Then(context =>
                {
                    context.Saga.ItemId = context.Message.ProductItemId;
                    context.Saga.Quantity = context.Message.Quantity;
                    context.Saga.Received = DateTimeOffset.UtcNow;
                    context.Saga.LastUpdated = context.Saga.Received;
                })
                .TransitionTo(Accepted)
                .Catch<Exception>(ex => ex
                    .Then(context =>
                    {
                        context.Saga.ErrorMessage = context.Exception.Message;
                        context.Saga.LastUpdated = DateTimeOffset.UtcNow;
                    })
                    .TransitionTo(Faulted))
        );
    }

    private void ConfigureAccepted()
    {
        During(Accepted,
            Ignore(PurchaseRequested),
            When(StockDecreasedEvent)
                .Then(context =>
                {
                    context.Saga.LastUpdated = DateTimeOffset.UtcNow;
                })
                .TransitionTo(StockGranted),
            When(GrantItemsFaulted)
                .Then(context =>
                {
                    context.Saga.ErrorMessage = context.Message.Exceptions[0].Message;
                    context.Saga.LastUpdated = DateTimeOffset.UtcNow;
                })
                .TransitionTo(Faulted)
        );
    }
    private void ConfigureStockGranted()
    {
        During(StockGranted,
            When(PaymentSucceededEvent)
                .Then(context =>
                {
                    context.Saga.LastUpdated = DateTimeOffset.UtcNow;
                })
                .TransitionTo(Completed),
         When(PaymentFailedEvent)
            .ThenAsync(async context =>
            {
                context.Saga.ErrorMessage = context.Message.Reason;
                context.Saga.LastUpdated = DateTimeOffset.UtcNow;

                await context.Publish(new StockIncreased(context.Saga.ItemId,context.Saga.Quantity,context.Saga.CorrelationId));
            })
            .TransitionTo(Faulted)
        );
    }
    private void ConfigureFaulted()
    {
        During(Faulted,
            Ignore(PurchaseRequested),
            Ignore(StockDecreasedEvent),
            When(StockIncreasedEvent)
                .Then(context =>
                {
                    context.Saga.Compensated = true;
                    context.Saga.LastUpdated = DateTimeOffset.UtcNow;
                })
                .TransitionTo(Completed)
        );
    }

    private void ConfigureCompleted()
    {
        During(Completed,
            Ignore(PurchaseRequested),
            Ignore(StockDecreasedEvent)
        );
    }

    private void ConfigureAny()
    {
        DuringAny(
            When(GetPurchaseState)
                .Respond(x => x.Saga)
        );
    }
}