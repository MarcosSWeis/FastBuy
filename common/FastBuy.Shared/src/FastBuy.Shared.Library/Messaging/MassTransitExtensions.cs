using FastBuy.Shared.Library.Configurations;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
namespace FastBuy.Shared.Library.Messaging
{
    public static class MassTransitExtensions
    {
        public static IServiceCollection AddMessageBroker(
            this IServiceCollection services,
            Assembly? consumerAssembly = null,
            Action<IRetryConfigurator>? retryConfigurator = null
            )
        {
            //Masstrnasit & RabbitMq  
            services.AddMassTransit(configure =>
            {
                if (consumerAssembly is not null)
                    configure.AddConsumers(consumerAssembly);

                configure.SetUpRabbitMq(retryConfigurator);

            });

            return services;
        }


        public static void SetUpRabbitMq(
            this IBusRegistrationConfigurator configure,
             Action<IRetryConfigurator>? retryConfigurator = null
            )
        {
            configure.UsingRabbitMq((context,configurator) =>
            {
                var configuration = context.GetService<IConfiguration>();

                var serviceSettings = configuration?.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>() ??
                                      throw new InvalidOperationException($"{nameof(ServiceSettings)} configuration is missing");

                var rabbitMQSettings = configuration.GetSection(nameof(BrokerSettings)).Get<BrokerSettings>() ??
                                       throw new InvalidOperationException($"rabbitMQSettings configuration is missing");

                configurator.Host(rabbitMQSettings.Host ?? throw new InvalidOperationException("RabbitMq host in not defined or configured."));

                configurator.ConfigureEndpoints(
                  context,
                  new KebabCaseEndpointNameFormatter(
                     serviceSettings.ServiceName,
                      includeNamespace: false
                      )
                  );

                if (retryConfigurator is null)
                    retryConfigurator = (retryConfigurator) => retryConfigurator.Interval(3,TimeSpan.FromSeconds(4)); //3 reintentos cada 4 segundos


                configurator.UseMessageRetry(retryConfigurator);
            });
        }
    }
}
