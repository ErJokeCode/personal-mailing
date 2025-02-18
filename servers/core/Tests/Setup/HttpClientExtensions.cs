using System.Net.Http.Json;

namespace Core.Tests.Setup;

public static class HttpClientExtensions
{
    public static async Task<TReturn> PostFromJsonAsync<TReturn, TValue>(this HttpClient httpClient, string requestUri, TValue value)
    {
        var response = await httpClient.PostAsJsonAsync(requestUri, value);
        return (await response.Content.ReadFromJsonAsync<TReturn>())!;
    }
}