using Subs.Core.Services.Worker;

namespace Subs.Worker.Workers;

public class ClientWorker(ILogger<ClientWorker> logger, IServiceProvider serviceProvider) : BackgroundService
{
    private readonly ILogger<ClientWorker> _logger = logger;
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var start = DateTime.UtcNow;
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var clientWorker = scope.ServiceProvider.GetRequiredService<ClientWorkerService>();
                await clientWorker.ConsumeAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("ClientWorker canceled.");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing clients.");
            }

            var elapsed = DateTime.UtcNow - start;
            var delay = TimeSpan.FromSeconds(2) - elapsed;
            if (delay > TimeSpan.Zero)
                await Task.Delay(delay, stoppingToken);
        }
    }
}