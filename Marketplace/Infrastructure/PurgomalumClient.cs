using Microsoft.AspNetCore.WebUtilities;

namespace Marketplace.Infrastructure;

public class PurgomalumClient
{
    private readonly HttpClient _httpClient;

    public PurgomalumClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public bool CheckForProfanity(string text) => CheckForProfanityAsync(text).GetAwaiter().GetResult();

    private async Task<bool> CheckForProfanityAsync(string text)
    {
        var result = await _httpClient.GetAsync(
            QueryHelpers.AddQueryString(
                "https://www.purgomalum.com/service/containsprofanity", "text", text));

        var value = await result.Content.ReadAsStringAsync();
        return bool.Parse(value);
    }
}
