using DOMAIN;
using Microsoft.EntityFrameworkCore;

namespace API.Config
{
    public static class DbContextRegistration
    {
        public static IServiceCollection AddTripestDbContext(
            this IServiceCollection services, 
            IConfiguration configuration)
        {
            var conn = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(conn))
            {
                throw new Exception("Missing default connection in configuration");
            }

            services.AddDbContext<TripnestDbContext>(options =>
            {
                var serverVersion = new MySqlServerVersion(new Version(8, 0, 33));
                options.UseMySql(conn, serverVersion, mySqlOptionsAction: sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null);
                });
            });

            return services;
        }
    }
}
