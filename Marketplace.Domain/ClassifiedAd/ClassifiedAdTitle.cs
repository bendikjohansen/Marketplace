using System.Text.RegularExpressions;

namespace Marketplace.Domain.ClassifiedAd;

public record ClassifiedAdTitle
{
    public static ClassifiedAdTitle FromString(string title)
    {
        CheckValidity(title);
        return new(title);
    }

    public static ClassifiedAdTitle FromHtml(string htmlTitle)
    {
        var supportedTagsReplaced = htmlTitle
            .Replace("<i>", "*")
            .Replace("</i>", "*")
            .Replace("<br>", "*")
            .Replace("</br>", "*");
        var value = Regex.Replace(supportedTagsReplaced, "<.*?>", string.Empty);

        CheckValidity(value);
        return new ClassifiedAdTitle(value);
    }

    public string Value { get; private set; }

    internal ClassifiedAdTitle(string value) => Value = value;

    // For persistence
    private ClassifiedAdTitle()
    {
    }


    private static void CheckValidity(string value)
    {
        if (value.Length > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Title cannot be longer than 100 characters");
        }
    }

    public static implicit operator string(ClassifiedAdTitle self) => self.Value;
}
