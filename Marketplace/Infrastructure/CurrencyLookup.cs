using Marketplace.Domain.Shared;

namespace Marketplace.Infrastructure;

public class CurrencyLookup : ICurrencyLookup
{
    public CurrencyDetails FindCurrency(string currencyCode) =>
        currencyCode switch
        {
            "USD" => Currency("USD"),
            "NOK" => Currency("NOK"),
            "EUR" => Currency("EUR"),
            _ => CurrencyDetails.None
        };

    private static CurrencyDetails Currency(string code) =>
        new() {CurrencyCode = code, DecimalPlaces = 2, InUse = true};
}
