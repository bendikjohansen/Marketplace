using Marketplace.Domain.Shared;

namespace Marketplace.Domain.UserProfile;

public record DisplayName
{
    public string Value { get; private set; }

    internal DisplayName(string value) => Value = value;

    public static DisplayName FromString(string displayName, CheckTextForProfanity hasProfanity)
    {
        if (string.IsNullOrEmpty(displayName))
        {
            throw new ArgumentNullException(nameof(displayName));
        }

        if (hasProfanity(displayName))
        {
            throw new DomainExceptions.ProfanityFound(displayName);
        }

        return new DisplayName(displayName);
    }

    public static implicit operator string(DisplayName displayName) => displayName.Value;

    // Satisfy the serialization requirements
    private DisplayName()
    {
    }
}
