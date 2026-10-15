using IntelliLoop.Common.Constants;
using IntelliLoop.Infrastructure.Data;
using IntelliLoop.Infrastructure.Data.Identity;
using Microsoft.AspNetCore.Identity;

namespace IntelliLoop.Web.Extensions
{
    public static class IdentityExtensions
    {
        public static IServiceCollection AddIdentityOptions(
            this IServiceCollection services)
        {
            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = AccountConstants.MinimumPasswordLength;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredUniqueChars = 1;
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            })
                .AddEntityFrameworkStores<IntelliLoopDbContext>()
                .AddDefaultTokenProviders();
            return services;
        }
    }
}
