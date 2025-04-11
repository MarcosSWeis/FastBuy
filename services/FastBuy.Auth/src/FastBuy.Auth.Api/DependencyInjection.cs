using FastBuy.Auth.Api.Entity;
using FastBuy.Auth.Api.Persistence;
using FastBuy.Auth.Api.Settings;
using FastBuy.Shared.Library.Configurations;
using FastBuy.Shared.Library.Repository.Implementations;
using Microsoft.AspNetCore.Identity;

namespace FastBuy.Auth.Api
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services,IConfiguration configuration)
        {
            var dbProvider = configuration.GetSection(nameof(DataBaseProvider)).Get<DataBaseProvider>()
              ?? throw new InvalidOperationException($"Falto definir el {nameof(DataBaseProvider)}.Provider en el appsettings.");

            var identityServiceSettings = configuration.GetSection(nameof(IdentityServerSettings)).Get<IdentityServerSettings>()
           ?? throw new InvalidOperationException($"Falto definir el {nameof(IdentityServerSettings)} en el appsettings.");

            var dataBase = DatabaseFactory.CreateDataBase<ApplicationDbContext>(dbProvider.Provider);

            dataBase.Configure(services,configuration);

            services.Configure<AuthSettings>(configuration.GetSection(nameof(AuthSettings)))
                .AddIdentity<ApplicationUser,ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddDefaultUI();



            //Identity service Configure register
            services.AddIdentityServer(options =>
            {
                //hanilita la registracion de distintos tipos de suscesos
                options.Events.RaiseSuccessEvents = true;//registra eventos exitos token emitidos registros de sesion
                options.Events.RaiseFailureEvents = true;//registra eventos fallidos como intentos de auth fallidos
                options.Events.RaiseErrorEvents = true;//registra lso enventos de error
            })
             .AddAspNetIdentity<ApplicationUser>()
             .AddInMemoryApiScopes(identityServiceSettings.ApiScopes)
             .AddInMemoryApiResources(identityServiceSettings.ApiResources)
             .AddInMemoryClients(identityServiceSettings.Clients)
             .AddInMemoryIdentityResources(identityServiceSettings.IdentityResources)
             .AddDeveloperSigningCredential();

            //añado la autenticacion a este mismo ms
            services.AddLocalApiAuthentication();

            //opccional
            services.Configure<IdentityOptions>(options =>
            {
                options.SignIn.RequireConfirmedEmail = false;
            });

            return services;
        }
    }
}
