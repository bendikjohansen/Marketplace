namespace Marketplace.Framework;

public abstract class Entity<TId> : IInternalEventHandler
{
    private readonly Action<object> _applier;
    protected Entity(Action<object> applier) => _applier = applier;
    public TId Id { get; protected set; }

    protected void Apply(object @event)
    {
        When(@event);
        _applier(@event);
    }

    protected abstract void When(object @event);
    void IInternalEventHandler.Handle(object @event) => When(@event);
}
