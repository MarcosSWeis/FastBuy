using FastBuy.Auth.Api.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FastBuy.Auth.Api.Persistence
{
    public class ApplicationDbContext :IdentityDbContext<ApplicationUser,ApplicationRole,string>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //fluent api
            builder.Entity<ApplicationUser>(x => x.ToTable("Users"));
            builder.Entity<ApplicationRole>(x => x.ToTable("Roles"));
            builder.Entity<IdentityUserRole<string>>(x => x.ToTable("UserRoles"));

            base.OnModelCreating(builder);
        }
    }
}
