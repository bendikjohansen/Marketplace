using Marketplace.Infrastructure;

namespace Marketplace;

public class EventStoreService : IHostedService
{
    private readonly ProjectionManager _subscription;

    public EventStoreService(ProjectionManager subscription)
    {
        _subscription = subscription;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _subscription.Start();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
