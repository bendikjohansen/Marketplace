namespace Marketplace.Domain.ClassifiedAd;

public record ClassifiedAdId
{
    public readonly Guid Value;

    public ClassifiedAdId(Guid value)
    {
        if (value == default)
        {
            throw new ArgumentNullException(nameof(value), "Classified Ad ID cannot be empty");
        }

        Value = value;
    }

    public static implicit operator Guid(ClassifiedAdId self) => self.Value;
    public override string ToString() => Value.ToString();
}
