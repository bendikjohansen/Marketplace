using EventStore.Client;

namespace Marketplace.Framework;

public interface ICheckpointStore
{
    Task<Position> GetCheckpoint();
    Task StoreCheckpoint(Position position);
}
