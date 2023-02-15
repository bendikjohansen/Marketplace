using EventStore.Client;
using Marketplace.Framework;
using Raven.Client.Documents.Session;

namespace Marketplace.Infrastructure;

public class RavenDbCheckpointStore : ICheckpointStore
{
    private readonly Func<IAsyncDocumentSession> _getSession;
    private readonly string _checkpointName;

    public RavenDbCheckpointStore(Func<IAsyncDocumentSession> getSession, string checkpointName)
    {
        _getSession = getSession;
        _checkpointName = checkpointName;
    }

    public async Task<Position> GetCheckpoint()
    {
        using var session = _getSession();
        var checkpoint = await session.LoadAsync<Checkpoint>(_checkpointName);
        return checkpoint?.Position ?? Position.Start;
    }

    public async Task StoreCheckpoint(Position position)
    {
        using var session = _getSession();

        var checkpoint = await session.LoadAsync<Checkpoint>(_checkpointName)
                         ?? new Checkpoint(_checkpointName, position);

        await session.StoreAsync(checkpoint);
        await session.SaveChangesAsync();
    }
}
