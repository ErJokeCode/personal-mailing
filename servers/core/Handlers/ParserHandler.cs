using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;

namespace Core.Handlers;

public static class ParserHandler
{
    public static async Task<T> GetFromParser<T>(string path, Dictionary<string, string> query)
    {
        HttpClient httpClient = new()
        {
            BaseAddress = new Uri("http://parser:8000"),
        };

        var response = await httpClient.GetAsync(QueryHelpers.AddQueryString(path, query));

        if (!response.IsSuccessStatusCode)
        {
            return default(T);
        }

        var serializerSettings = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };

        var result = await response.Content.ReadAsStringAsync();
        var obj = JsonSerializer.Deserialize<T>(result, serializerSettings);

        return obj;
    }
}
