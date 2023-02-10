using EventStore.Client;
using Marketplace.Projections;

namespace Marketplace.Infrastructure;

public class ProjectionManager
{
    private readonly EventStoreClient _client;
    private readonly IProjection[] _projections;
    private readonly ILogger<ProjectionManager> _logger;

    public ProjectionManager(EventStoreClient client,
        IEnumerable<IProjection> projections,
        ILogger<ProjectionManager> logger)
    {
        _client = client;
        _logger = logger;
        _projections = projections.ToArray();
    }

    public Task Start() => _client.SubscribeToAllAsync(FromAll.Start, EventAppeared, true);

    private Task EventAppeared(StreamSubscription subscription, ResolvedEvent resolvedEvent,
        CancellationToken cancellationToken)
    {
        if (resolvedEvent.Event.EventType.StartsWith("$")) return Task.CompletedTask;

        var @event = resolvedEvent.Deserialize();
        _logger.LogInformation("Projecting event {type}", @event.GetType().Name);
        return Task.WhenAll(_projections.Select(x => x.Project(@event)));
    }
}
