using System.Text.RegularExpressions;

namespace Marketplace.Domain;

public record ClassifiedAdTitle
{
    public static ClassifiedAdTitle FromString(string title) => new(title);

    public static ClassifiedAdTitle FromHtml(string htmlTitle)
    {
        var supportedTagsReplaced = htmlTitle
            .Replace("<i>", "*")
            .Replace("</i>", "*")
            .Replace("<br>", "*")
            .Replace("</br>", "*");

        return new ClassifiedAdTitle(Regex.Replace(supportedTagsReplaced, "<.*?>", string.Empty));
    }

    private string _value;

    private ClassifiedAdTitle(string value)
    {
        if (value.Length > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Title cannot be longer than 100 characters");
        }

        _value = value;
    }

    public static implicit operator string(ClassifiedAdTitle self) => self._value;
}
