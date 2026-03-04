using APPLICATION.Interfaces.Auth;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace WORKER.Services
{
    public class RefreshTokenCleanupService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<RefreshTokenCleanupService> _logger;

        public RefreshTokenCleanupService(IServiceScopeFactory scopeFactory,
            ILogger<RefreshTokenCleanupService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("RefreshTokenCleanupService is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var repo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                    var deleted = await repo.DeleteExpiredRefreshTokenAsync();

                    _logger.LogInformation("RefreshTokenCleanupService deleted {Count} expired refresh tokens.", deleted);
                }
                catch(OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    // graceful shutdown
                    _logger.LogInformation("RefreshTokenCleanupService cancellation requested.");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while cleaning up expired refresh tokens.");
                    // nếu muốn, chờ ngắn để tránh tight-loop khi lỗi lặp lại:
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken).ContinueWith(_ => { });
                }

                try
                {
                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
            }

            _logger.LogInformation("RefreshTokenCleanupService is stopping.");
        }
    }
}
