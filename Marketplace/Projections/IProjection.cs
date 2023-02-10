namespace Marketplace.Projections;

public interface IProjection
{
    Task Project(object @event);
}
