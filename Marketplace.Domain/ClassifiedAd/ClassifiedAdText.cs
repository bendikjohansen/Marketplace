namespace Marketplace.Domain.ClassifiedAd;

public record ClassifiedAdText
{
    public string Value { get; private set; }

    internal ClassifiedAdText(string value) => Value = value;

    // For persistence
    private ClassifiedAdText()
    {
    }

    public static ClassifiedAdText FromString(string text) => new(text);

    public static implicit operator string(ClassifiedAdText self) => self.Value;
}
