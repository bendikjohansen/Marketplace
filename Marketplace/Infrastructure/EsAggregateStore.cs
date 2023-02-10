using System.Text;
using EventStore.Client;
using Marketplace.Framework;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Marketplace.Infrastructure;

public class EsAggregateStore : IAggregateStore
{
    private readonly EventStoreClient _connection;

    public EsAggregateStore(EventStoreClient connection)
    {
        _connection = connection;
    }

    public async Task Save<T, TId>(T aggregate) where T : AggregateRoot<TId>
    {
        _ = aggregate ?? throw new ArgumentNullException(nameof(aggregate));

        var changes = aggregate.GetChanges()
            .Select(@event => new EventData(
                eventId: Uuid.NewUuid(),
                type: @event.GetType().Name,
                data: Serialize(@event),
                metadata: Serialize(new { ClrType = @event.GetType().AssemblyQualifiedName })
            ))
            .ToArray();

        if (!changes.Any()) return;

        var streamName = GetStreamName<T, TId>(aggregate);
        await _connection.AppendToStreamAsync(streamName, StreamState.Any, changes);

        aggregate.ClearChanges();
    }

    public async Task<T> Load<T, TId>(TId aggregateId) where T : AggregateRoot<TId>
    {
        _ = aggregateId ?? throw new ArgumentNullException(nameof(aggregateId));

        var stream = GetStreamName<T, TId>(aggregateId);
        var aggregate = Activator.CreateInstance<T>();
        var page = _connection.ReadStreamAsync(Direction.Forwards, stream, StreamPosition.Start);
        aggregate.Load((await page.ToListAsync()).Select(resolvedEvent => resolvedEvent.Deserialize()).ToArray());

        return aggregate;
    }

    public async Task<bool> Exists<T, TId>(TId aggregateId) where T : AggregateRoot<TId>
    {
        var stream = GetStreamName<T, TId>(aggregateId);
        var result = _connection.ReadStreamAsync(Direction.Forwards, stream, StreamPosition.Start);
        return await result.ReadState != ReadState.StreamNotFound;
    }

    private static byte[] Serialize(object data)
        => Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data));

    private static string GetStreamName<T, TId>(TId aggregateId)
        => $"{typeof(T).Name}-{aggregateId}";

    private static string GetStreamName<T, TId>(T aggregate) where T : AggregateRoot<TId>
        => $"{typeof(T).Name}-{aggregate.Id}";

}

public class EventMetadata
{
    public string ClrType { get; init; }
}
