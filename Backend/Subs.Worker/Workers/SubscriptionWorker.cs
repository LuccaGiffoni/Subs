using Subs.Core.Services.Worker;

namespace Subs.Worker.Workers;

public class SubscriptionWorker(ILogger<SubscriptionWorker> logger, IServiceProvider serviceProvider) : BackgroundService
{
    private readonly ILogger<SubscriptionWorker> _logger = logger;
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var start = DateTime.UtcNow;
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var subscriptionWorker = scope.ServiceProvider.GetRequiredService<SubscriptionWorkerService>();
                await subscriptionWorker.ConsumeAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("SubscriptionWorker canceled.");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing subscriptions.");
            }

            var elapsed = DateTime.UtcNow - start;
            var delay = TimeSpan.FromSeconds(2) - elapsed;
            if (delay > TimeSpan.Zero)
                await Task.Delay(delay, stoppingToken);
        }
    }
}