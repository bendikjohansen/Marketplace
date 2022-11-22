using Marketplace.Framework;

namespace Marketplace.Domain.ClassifiedAd;

public class Picture : Entity<PictureId>
{
    internal PictureSize Size { get; set; }
    internal Uri Location { get; set; }
    internal int Order { get; set; }

    public void Resize(PictureSize newSize) => Apply(new Events.ClassifiedAdPictureResized
    {
        PictureId = Id,
        Height = newSize.Height,
        Width = newSize.Width
    });

    protected override void When(object @event)
    {
        switch (@event)
        {
            case Events.PictureAddedToAClassifiedAd e:
                Id = new PictureId(e.PictureId);
                Location = new Uri(e.Url);
                Size = new PictureSize {Width = e.Width, Height = e.Height};
                Order = e.Order;
                break;
            case Events.ClassifiedAdPictureResized e:
                Size = new PictureSize {Width = e.Width, Height = e.Height};
                break;
        }
    }

    public Picture(Action<object> applier) : base(applier)
    {
    }
}

public class PictureId
{
    public readonly Guid Value;
    public PictureId(Guid value)
    {
        if (value == default)
        {
            throw new ArgumentNullException(nameof(value), "Picture ID cannot be empty");
        }

        Value = value;
    }
    public static implicit operator Guid(PictureId self) => self.Value;
    public override string ToString() => Value.ToString();
}

public record PictureSize
{
    public int Width { get; internal set; }
    public int Height { get; internal set; }

    public PictureSize(int width, int height)
    {
        if (width <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(width), "Picture width must be a positive number");
        }

        if (height <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(height), "Picture height must be a positive number");
        }

        Width = width;
        Height = height;
    }

    internal PictureSize()
    {
    }
}

public static class PictureRules
{
    public static bool HasCorrectSize(this Picture? picture) => picture != null
                                                                && picture.Size.Width >= 800
                                                                && picture.Size.Height >= 600;
}
