using EventStore.Client;
using Marketplace.Domain.ClassifiedAd;
using Marketplace.Framework;
using Marketplace.Projections;

namespace Marketplace.Infrastructure;

public class ProjectionManager
{
    private readonly EventStoreClient _client;
    private readonly IProjection[] _projections;
    private readonly ILogger<ProjectionManager> _logger;
    private readonly ICheckpointStore _checkpointStore;

    public ProjectionManager(EventStoreClient client,
        IEnumerable<IProjection> projections,
        ILogger<ProjectionManager> logger,
        ICheckpointStore checkpointStore)
    {
        _client = client;
        _logger = logger;
        _checkpointStore = checkpointStore;
        _projections = projections.ToArray();
    }

    public async Task Start()
    {
        var position = await _checkpointStore.GetCheckpoint();
        await _client.SubscribeToAllAsync(FromAll.After(position), EventAppeared, true);
    }

    private async Task EventAppeared(StreamSubscription subscription, ResolvedEvent resolvedEvent,
        CancellationToken cancellationToken)
    {
        if (resolvedEvent.Event.EventType.StartsWith("$")) return;

        var @event = resolvedEvent.Deserialize();
        _logger.LogInformation("Projecting event {type}", @event.GetType().Name);
        await Task.WhenAll(_projections.Select(x => x.Project(@event)));

        await _checkpointStore.StoreCheckpoint(resolvedEvent.OriginalPosition.Value);
    }
}
