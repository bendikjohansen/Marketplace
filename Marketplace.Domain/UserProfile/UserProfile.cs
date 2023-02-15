using Marketplace.Domain.ClassifiedAd;
using Marketplace.Framework;

namespace Marketplace.Domain.UserProfile;

public class UserProfile : AggregateRoot<UserId>
{
    public FullName FullName { get; private set; }
    public DisplayName DisplayName { get; private set; }
    public string PhotoUrl { get; private set; }

    public UserProfile(UserId id, FullName fullName, DisplayName displayName)
        => Apply(new Events.UserRegistered(id, fullName, displayName));
    public void UpdateFullName(FullName fullName) => Apply(new Events.UserFullNameUpdated(Id, fullName));
    public void UpdateDisplayName(DisplayName displayName) => Apply(new Events.UserDisplayNameUpdated(Id, displayName));
    public void UpdateProfilePicture(Uri photoUrl) => Apply(new Events.ProfilePictureUploaded(Id, photoUrl.ToString()));

    protected override void When(object @event)
    {
        switch (@event)
        {
            case Events.UserRegistered e:
                Id = new UserId(e.UserId);
                FullName = new FullName(e.FullName);
                DisplayName = new DisplayName(e.DisplayName);
                break;
            case Events.UserFullNameUpdated e:
                FullName = new FullName(e.FullName);
                break;
            case Events.UserDisplayNameUpdated e:
                DisplayName = new DisplayName(e.DisplayName);
                break;
            case Events.ProfilePictureUploaded e:
                PhotoUrl = e.PhotoUrl;
                break;
        }
    }

    protected override void EnsureValidState()
    {
    }

    // Satisfy the serialization requirements
    public UserProfile() {}
}
