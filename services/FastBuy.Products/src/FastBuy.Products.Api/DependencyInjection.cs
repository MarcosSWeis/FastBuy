using FastBuy.Products.Entities;
using FastBuy.Products.Services.Abstractions;
using FastBuy.Products.Services.Implementations;
using FastBuy.Shared.Library.Configurations;
using FastBuy.Shared.Library.Messaging;
using FastBuy.Shared.Library.Repository.Abstractions;
using FastBuy.Shared.Library.Repository.Implementations;
using FastBuy.Shared.Library.Security;

namespace FastBuy.Products.Api
{
    public static class DependencyInjection
    {

        public static IServiceCollection AddApplicationServices(this IServiceCollection services,IConfiguration configuration)
        {
            //Option pattern
            services.Configure<ServiceSettings>(configuration.GetSection(nameof(ServiceSettings)));
            //Registering Services     
            var dbProvider = configuration.GetSection(nameof(DataBaseProvider)).Get<DataBaseProvider>()
                ?? throw new InvalidOperationException($"Falto definir el {nameof(DataBaseProvider)}.Provider en el appsettings.");

            IDataBase database = DatabaseFactory.CreateDataBase(dbProvider.Provider);

            database.Configure(services,configuration);

            var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>()
                ?? throw new InvalidOperationException($"Falto definir el {nameof(ServiceSettings)} en el appsettings.");

            database.RegisterRepositories<Product>(services,serviceSettings.ServiceName);

            services.AddMessageBroker(configuration)
                .AddJwtBearerAuthentication();

            services.AddScoped<IProductService,ProductService>();

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
