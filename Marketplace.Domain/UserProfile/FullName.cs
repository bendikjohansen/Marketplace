namespace Marketplace.Domain.UserProfile;

public record FullName
{
    public string Value { get; private set; }

    internal FullName(string fullName) => Value = fullName;

    // Satisfy the serialization requirements
    private FullName()
    {
    }

    public static FullName FromString(string fullName)
    {
        if (string.IsNullOrEmpty(fullName))
        {
            throw new ArgumentNullException(nameof(fullName));
        }

        return new FullName(fullName);
    }

    public static implicit operator string(FullName self) => self.Value;
}
