using Microsoft.EntityFrameworkCore;
using IntelliLoop.Common.ExceptionMessages;
using IntelliLoop.Infrastructure.Data;

namespace IntelliLoop.Web.Extensions
{
    public static class DatabaseExtensions
    {
        public static IServiceCollection AddDatabase(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(Configuration.ConnectionStringNotFound);

            services.AddDbContext<IntelliLoopDbContext>(options =>
                options.UseSqlServer(connectionString));
            services.AddDatabaseDeveloperPageExceptionFilter();
            return services;
        }
    }
}
