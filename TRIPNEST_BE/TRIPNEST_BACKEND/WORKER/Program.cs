using APPLICATION.Interfaces.Auth;
using DOMAIN;
using INFRASTRUCTURE.Interfaces.Users;
using INFRASTRUCTURE.Repositories.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WORKER.Services;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // DbContext
        services.AddDbContext<TripnestDbContext>(opts => 
            opts.UseMySql(
                context.Configuration.GetConnectionString("DefaultConnection"),
                ServerVersion.AutoDetect(context.Configuration.GetConnectionString("DefaultConnection"))
            )
        );

        // DI bindings
        services.AddScoped<IUsersRepository, UserRepository>();

        // Worker services
        services.AddHostedService<RefreshTokenCleanupService>();
    })
    .Build();

await host.RunAsync();
