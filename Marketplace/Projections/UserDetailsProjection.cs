using Marketplace.Domain.UserProfile;

namespace Marketplace.Projections;

public class UserDetailsProjection : IProjection
{
    private readonly IList<ReadModels.UserDetails> _items;

    public UserDetailsProjection(IList<ReadModels.UserDetails> items) => _items = items;

    public Task Project(object @event)
    {
        switch (@event)
        {
            case Events.UserRegistered e:
                _items.Add(new ReadModels.UserDetails
                {
                    UserId = e.UserId,
                    DisplayName = e.DisplayName
                });
                break;
            case Events.UserDisplayNameUpdated e:
                UpdateItem(e.UserId, user => user.DisplayName = e.DisplayName);
                break;
        }
        return Task.CompletedTask;
    }

    private void UpdateItem(Guid id,
        Action<ReadModels.UserDetails> update)
    {
        var item = _items.FirstOrDefault(x => x.UserId == id);
        if (item == null) return;
        update(item);
    }
}
