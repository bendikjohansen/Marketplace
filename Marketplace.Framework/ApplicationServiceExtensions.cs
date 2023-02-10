namespace Marketplace.Framework;

public static class A0pplicationServiceExtensions
{
    public static async Task HandleUpdate<T, TId>(
        this IApplicationService service,
        IAggregateStore store,
        TId aggregateId,
        Action<T> operation) where T : AggregateRoot<TId>
    {
        var aggregate = await store.Load<T, TId>(aggregateId) ??
                        throw new InvalidOperationException($"Entity with id {aggregateId} cannot be found.");

        operation(aggregate);
        await store.Save<T, TId>(aggregate);
    }
}