using FastBuy.Orders.Api.StateMachines;
using FastBuy.Orders.Entities;
using FastBuy.Orders.Entities.Settings;
using FastBuy.Orders.Services.Consumers;
using FastBuy.Orders.Services.Exceptions;
using FastBuy.Shared.Library.Configurations;
using FastBuy.Shared.Library.Repository.Implementations;
using FastBuy.Shared.Library.Security;
using MassTransit;

namespace FastBuy.Orders.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        var dbProvider = configuration.GetSection(nameof(DataBaseProvider)).Get<DataBaseProvider>()
                  ?? throw new InvalidOperationException($"Falto definir el {nameof(DataBaseProvider)}.Provider en el appsettings.");

        var database = DatabaseFactory.CreateDataBase(dbProvider.Provider);
        database.Configure(services,configuration);
        database.RegisterRepositories<Order>(services,"Orders");
        database.RegisterRepositories<ProductItem>(services,"ProductItem");
        services.AddJwtBearerAuthentication();

        AddMassTransit(services,configuration);
        return services;
    }

    private static void AddMassTransit(IServiceCollection services,IConfiguration configuration)
    {
        services.AddMassTransit(configure =>
        {
            configure.UsingRabbitMq((context,cfg) =>
            {
                cfg.Host("localhost","/",h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.UseMessageRetry(retry =>
                {
                    retry.Interval(3,TimeSpan.FromSeconds(5));
                    retry.Ignore(typeof(UnknowItemExceptions));
                });

                cfg.ConfigureEndpoints(context);
            });

            configure.AddConsumers(typeof(ProductItemCreatedConsumer).Assembly);
            configure.AddSagaStateMachine<OrderStateMachine,OrderState>(sagaConfigurator =>
                {
                    sagaConfigurator.UseInMemoryOutbox();
                })
                .MongoDbRepository(r =>
                {
                    var serviceSettings = configuration.GetSection(nameof(ServiceSettings))
                        .Get<ServiceSettings>();
                    var mongoSettings = configuration.GetSection(nameof(MongoDbSettings))
                        .Get<MongoDbSettings>();

                    r.Connection = mongoSettings.ConnectionString;
                    r.DatabaseName = serviceSettings.ServiceName;
                });

        });

        var queueSettings = configuration.GetSection(nameof(QueueSettings)).Get<QueueSettings>();

    }
}