using Marketplace.Domain.Shared;

namespace Marketplace.Domain.ClassifiedAd;

public record Price : Money
{
    internal Price(decimal amount, string currencyCode) : base(amount, currencyCode)
    {
    }

    private Price(decimal amount, string currencyCode, ICurrencyLookup currencyLookup) : base(amount, currencyCode,
        currencyLookup)
    {
        if (amount < 0)
        {
            throw new ArgumentException("Price cannot be negative", nameof(amount));
        }
    }

    protected Price()
    {
    }

    public new static Price FromDecimal(decimal amount, string currency, ICurrencyLookup currencyLookup) =>
        new(amount, currency, currencyLookup);
}
