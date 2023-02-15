using EventStore.Client;

namespace Marketplace.Infrastructure;

public record Checkpoint(string Id, Position Position);
