
using IntelliLoop.Common.Constants;
using IntelliLoop.Infrastructure.Data;
using IntelliLoop.Infrastructure.Data.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace IntelliLoop.Web.Extensions
{
    public static class IdentityExtensions
    {
        public static IServiceCollection AddIdentityOptions(
            this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;

                options.Password.RequiredLength = AccountConstants.MinimumPasswordLength;

                options.Password.RequireDigit =
                    bool.Parse(configuration["Identity:Password:RequireDigit"]
                        ?? throw new InvalidOperationException("Require digit is not configured."));

                options.Password.RequireLowercase =
                    bool.Parse(configuration["Identity:Password:RequireLowercase"]
                        ?? throw new InvalidOperationException("Require lowercase is not configured."));

                options.Password.RequireUppercase =
                    bool.Parse(configuration["Identity:Password:RequireUppercase"]
                        ?? throw new InvalidOperationException("Require uppercase is not configured."));

                options.Password.RequireNonAlphanumeric =
                    bool.Parse(configuration["Identity:Password:RequireNonAlphanumeric"]
                        ?? throw new InvalidOperationException("Require non-alphanumeric is not configured."));

                options.Password.RequiredUniqueChars =
                    int.Parse(configuration["Identity:Password:RequiredUniqueChars"]
                        ?? throw new InvalidOperationException("Required unique characters is not configured."));

                options.Lockout.AllowedForNewUsers =
                    bool.Parse(configuration["Identity:Lockout:AllowedForNewUsers"]
                        ?? throw new InvalidOperationException("Allowed for new users is not configured."));

                options.Lockout.MaxFailedAccessAttempts =
                    int.Parse(configuration["Identity:Lockout:MaxFailedAccessAttempts"]
                        ?? throw new InvalidOperationException("Max failed access attempts is not configured."));

                options.Lockout.DefaultLockoutTimeSpan =
                    TimeSpan.Parse(configuration["Identity:Lockout:DefaultLockoutTimeSpan"]
                        ?? throw new InvalidOperationException("Lockout time span is not configured."));

                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
                //Didn't bother putting them in constants since I didn't implement Email system anyway,
                //so these are just for reference if I ever implement it.

            })
            .AddEntityFrameworkStores<IntelliLoopDbContext>()
            .AddDefaultTokenProviders();

            services.AddTransient<IEmailSender, NoOpEmailSender>();

            return services;
        }
    }
}
