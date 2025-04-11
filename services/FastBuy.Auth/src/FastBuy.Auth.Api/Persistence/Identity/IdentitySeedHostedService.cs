
using FastBuy.Auth.Api.Entity;
using FastBuy.Auth.Api.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace FastBuy.Auth.Api.Persistence.Identity
{
    public class IdentitySeedHostedService :IHostedService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IOptions<AuthSettings> _options;
        private readonly ILogger<IdentitySeedHostedService> _logger;

        public IdentitySeedHostedService(
            IServiceScopeFactory serviceScopeFactory,
            IOptions<AuthSettings> options,
            ILogger<IdentitySeedHostedService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var serviceProvider = scope.ServiceProvider;

            try
            {
                var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
                var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                _logger.LogInformation("Starting identity seeding...");

                await EnsureRolesExistAsync(roleManager);
                await EnsureAdminUserExistsAsync(userManager);

                _logger.LogInformation("Identity seeding completed successfully.");
            } catch (Exception ex)
            {
                _logger.LogError(ex,"An error occurred while seeding identity data.");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        private async Task EnsureRolesExistAsync(RoleManager<ApplicationRole> roleManager)
        {
            foreach (var role in new[] { Roles.Admin,Roles.Customer })
            {
                if (!await roleManager.RoleExistsAsync(role).ConfigureAwait(false))
                {
                    _logger.LogInformation("Creating role '{RoleName}'...",role);
                    await roleManager.CreateAsync(new ApplicationRole { Name = role }).ConfigureAwait(false);
                }
            }
        }

        private async Task EnsureAdminUserExistsAsync(UserManager<ApplicationUser> userManager)
        {
            var adminEmail = _options.Value.AdminUserEmail;
            var adminPassword = _options.Value.AdminUserPassword;

            var adminUser = await userManager.FindByEmailAsync(adminEmail).ConfigureAwait(false);

            if (adminUser == null)
            {
                _logger.LogInformation("Creating admin user '{AdminEmail}'...",adminEmail);

                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = _options.Value.FirstName,
                    LastName = _options.Value.LastName,
                    DocumentType = Enum.Parse<DocumentTypeEnum>(_options.Value.DocumentType),
                    DocumentNumber = _options.Value.DocumentNumber
                };

                var result = await userManager.CreateAsync(adminUser,adminPassword).ConfigureAwait(false);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser,Roles.Admin).ConfigureAwait(false);
                    _logger.LogInformation("Admin user '{AdminEmail}' created successfully.",adminEmail);
                } else
                {
                    _logger.LogError("Failed to create admin user '{AdminEmail}': {Errors}",adminEmail,string.Join(", ",result.Errors));
                }
            } else
            {
                _logger.LogInformation("Admin user '{AdminEmail}' already exists.",adminEmail);
            }
        }
    }
}