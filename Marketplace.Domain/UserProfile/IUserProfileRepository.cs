using Marketplace.Domain.ClassifiedAd;

namespace Marketplace.Domain.UserProfile;

public interface IUserProfileRepository
{
    Task<UserProfile?> Load(UserId id);
    Task Add(UserProfile entity);
    Task<bool> Exists(UserId id);
}
