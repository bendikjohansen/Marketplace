namespace Marketplace.Domain;

public record ClassifiedAdText
{
    public string Value { get; }

    internal ClassifiedAdText(string value) => Value = value;

    public static ClassifiedAdText FromString(string text) => new(text);

    public static implicit operator string(ClassifiedAdText self) => self.Value;
}
