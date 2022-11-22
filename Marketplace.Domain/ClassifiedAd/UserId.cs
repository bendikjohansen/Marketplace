namespace Marketplace.Domain.ClassifiedAd;

public record UserId
{
    public readonly Guid Value;

    public UserId(Guid value)
    {
        if (value == default)
        {
            throw new ArgumentNullException(nameof(value), "User id cannot be empty");
        }

        Value = value;
    }

    public static implicit operator Guid(UserId self) => self.Value;
    public override string ToString() => Value.ToString();
}
