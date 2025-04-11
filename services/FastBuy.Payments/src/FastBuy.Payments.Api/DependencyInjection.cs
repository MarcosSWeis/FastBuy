using FastBuy.Payments.Entities;
using FastBuy.Payments.Services;
using FastBuy.Payments.Services.Client;
using FastBuy.Payments.Services.Consumers;
using FastBuy.Payments.Services.Exceptions;
using FastBuy.Shared.Library.Configurations;
using FastBuy.Shared.Library.Messaging;
using FastBuy.Shared.Library.Repository.Implementations;
using FastBuy.Shared.Library.Security;
using MassTransit;

namespace FastBuy.Payments.Api
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services,IConfiguration configuration)
        {
            var dbProvider = configuration.GetSection(nameof(DataBaseProvider)).Get<DataBaseProvider>()
          ?? throw new InvalidOperationException($"Falto definir el {nameof(DataBaseProvider)}.Provider en el appsettings.");

            var database = DatabaseFactory.CreateDataBase(dbProvider.Provider);
            database.Configure(services,configuration);
            database.RegisterRepositories<Payment>(services,"Payments");
            database.RegisterRepositories<OrderItem>(services,nameof(OrderItem));
            services.AddMessageBroker(typeof(OrderItemCreatedConsumer).Assembly,
                            retryConfiguration =>
                            {
                                retryConfiguration.Interval(3,TimeSpan.FromSeconds(4));
                                retryConfiguration.Ignore(typeof(UnknowItemExceptions));
                            })
                            .AddJwtBearerAuthentication();

            services.AddHttpClient<OrderClient>(client =>
            {
                client.BaseAddress = new Uri(configuration.GetSection("Urls:OrdersUrl").Value
                                             ?? throw new ApplicationException("No está definida la url de orders"));
            });


            services.AddScoped<IPaymentServices,PaymentServices>();

            return services;
        }


        public static IServiceCollection AddPresentation(this IServiceCollection services)
        {
            //*****          registros de servicio          *******
            // Add services to the container.
            services.AddControllers(options =>
            {
                options.SuppressAsyncSuffixInActionNames = false;
            });


            //****          registro los servicios de Swagger        ****
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            return services;
        }
    }


}
