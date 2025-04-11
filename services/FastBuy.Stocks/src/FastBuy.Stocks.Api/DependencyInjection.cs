using FastBuy.Shared.Library.Configurations;
using FastBuy.Shared.Library.Messaging;
using FastBuy.Shared.Library.Repository.Implementations;
using FastBuy.Shared.Library.Security;
using FastBuy.Stocks.Entities;
using FastBuy.Stocks.Services.Abstractions;
using FastBuy.Stocks.Services.Consumers;
using FastBuy.Stocks.Services.Exceptions;
using FastBuy.Stocks.Services.Implementations;
using MassTransit;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;

namespace FastBuy.Stocks.Api
{
    internal static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services,IConfiguration configuration)
        {
            //Option pattern
            services.Configure<ServiceSettings>(configuration.GetSection(nameof(ServiceSettings)));

            var dbProvider = configuration.GetSection(nameof(DataBaseProvider)).Get<DataBaseProvider>()
               ?? throw new InvalidOperationException($"Falto definir el {nameof(DataBaseProvider)}.Provider en el appsettings.");

            var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>()
               ?? throw new InvalidOperationException($"Falto definir el {nameof(ServiceSettings)} en el appsettings.");

            var dataBase = DatabaseFactory.CreateDataBase(dbProvider.Provider);
            dataBase.Configure(services,configuration);

            dataBase.RegisterRepositories<StockItem>(services,serviceSettings.ServiceName);
            dataBase.RegisterRepositories<ProductItem>(services,nameof(ProductItem));

            services.AddMessageBroker(typeof(ProductItemCreatedConsumer).Assembly,
                retryConfiguration =>
                {
                    retryConfiguration.Interval(3,TimeSpan.FromSeconds(4));
                    retryConfiguration.Ignore(typeof(NonExistentProductException));
                })
                .AddJwtBearerAuthentication();

            #region Resilience code

            //Timeout policy (evita que la solicitudes se queden colgadas mucho tiempo)
            var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(
                TimeSpan.FromSeconds(2), //segundos para cancelar
                TimeoutStrategy.Pessimistic //cuand oel codigo no admite cancelaciones o no lo sabemos
                );

            //Reintentos automaticos con jitter (evita sobrecarga en casos de fallos masivos)
            var randomJitter = new Random();
            var retryPolicy = HttpPolicyExtensions
                              .HandleTransientHttpError()
                              .Or<TimeoutRejectedException>()
                              .WaitAndRetryAsync(
                                retryCount: 5,
                                sleepDurationProvider: retryAttempt =>
                                TimeSpan.FromSeconds(Math.Pow(2,retryAttempt)) + TimeSpan.FromMicroseconds(randomJitter.Next(0,100)),

                                //no necesaria en prod
                                onRetry: (outCome,timeSpan,retyAttempt,context) =>
                                {
                                    Console.WriteLine($"[RETRY] Intento {retyAttempt} fallido. Reintentando en {timeSpan.TotalSeconds} segundos..");
                                }
                                );

            //Circuit braeker policy pattern  (evita sibrecargar un servicio que esta fallando repetidamente)

            var circuitBraekePolicy = Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .Or<TimeoutRejectedException>()
                .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 2, //cuantos eventos son permitidos antes del fallo
                durationOfBreak: TimeSpan.FromSeconds(10), //cuanto dura el break

                //solo en desarrollo
                onBreak: (outcome,timespan) =>
                {
                    Console.WriteLine($"[CIRCUIT BREAKER] Se ha abierto el circuito por {timespan.TotalSeconds} segundos debido a {outcome.Exception.Message}");
                },
                onReset: () =>
                {
                    Console.WriteLine($"[CIRCUIT BREAKER] Circuito cerrado");
                }
                );


            #endregion

            //cominicacion sincrona no se usa ()
            #region comunicacion sincrona

            //client registation
            //services.AddHttpClient<ProdutcClient>(client =>
            //{
            //    client.BaseAddress = new Uri(configuration.GetSection("Urls:ProductsUrl").Value
            //                                 ?? throw new ApplicationException("No está definida la url de prouctos"));
            //})
            //.AddPolicyHandler(retryPolicy)
            //.AddPolicyHandler(timeoutPolicy)
            //.AddPolicyHandler(circuitBraekePolicy);
            #endregion 


            //registro servicios de proyectos      
            services.AddScoped<IStockService,StockService>();

            return services;
        }


        public static IServiceCollection AddPresentation(this IServiceCollection services)
        {

            services.AddControllers(options =>
            {
                options.SuppressAsyncSuffixInActionNames = false;

            });

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            return services;
        }
    }
}