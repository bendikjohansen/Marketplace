using System.Globalization;

namespace Marketplace.Domain;

public record Money
{
    public const string DefaultCurrency = "EUR";

    public static Money FromDecimal(decimal amount, string currency, ICurrencyLookup currencyLookup) =>
        new(amount, currency, currencyLookup);

    public static Money FromString(string amount, string currency, ICurrencyLookup currencyLookup) =>
        new(decimal.Parse(amount, CultureInfo.InvariantCulture), currency, currencyLookup);

    protected Money(decimal amount, string currencyCode, ICurrencyLookup currencyLookup)
    {
        if (string.IsNullOrEmpty(currencyCode))
        {
            throw new ArgumentNullException(nameof(currencyCode), "Currency code must be specified");
        }

        var currency = currencyLookup.FindCurrency(currencyCode);
        if (!currency.InUse)
        {
            throw new ArgumentException($"Currency {currencyCode} is not valid");
        }

        if (decimal.Round(amount, currency.DecimalPlaces) != amount)
        {
            throw new ArgumentOutOfRangeException(nameof(amount),
                $"Amount cannot have more than {currency.DecimalPlaces} decimals");
        }

        Amount = amount;
        CurrencyCode = currencyCode;
    }

    protected Money(decimal amount, string currencyCode)
    {
        Amount = amount;
        CurrencyCode = currencyCode;
    }

    public decimal Amount { get; }
    public string CurrencyCode { get; }

    private Money Add(Money summand)
    {
        if (CurrencyCode != summand.CurrencyCode)
        {
            throw new CurrencyMismatchException("Cannot sum amounts with different currencies");
        }

        return new Money(Amount + summand.Amount, CurrencyCode);
    }

    private Money Subtract(Money summand)
    {
        if (CurrencyCode != summand.CurrencyCode)
        {
            throw new CurrencyMismatchException("Cannot sum amounts with different currencies");
        }

        return new Money(Amount - summand.Amount, CurrencyCode);
    }

    public static Money operator +(Money summand1, Money summand2) => summand1.Add(summand2);
    public static Money operator -(Money minuend, Money subtrahend) => minuend.Subtract(subtrahend);
}

public class CurrencyMismatchException : Exception
{
    public CurrencyMismatchException(string message) : base(message)
    {
    }
}
