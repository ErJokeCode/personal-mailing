using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Abstractions.Parser;
using Core.Models;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Core.Infrastructure.Services;

public class Parser : IParser
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly string _parserUrl;

    public Parser(IOptions<ParserOptions> options, IHttpClientFactory clientFactory)
    {
        _parserUrl = options.Value.ParserUrl;
        _clientFactory = clientFactory;

        _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        };
    }

    public async Task<T?> GetAsync<T>(string path, Dictionary<string, string?> query)
    {
        var client = _clientFactory.CreateClient();
        client.BaseAddress = new Uri(_parserUrl);

        var response = await client.GetAsync(QueryHelpers.AddQueryString(path, query));

        if (!response.IsSuccessStatusCode)
        {
            return default;
        }

        var result = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(result, _jsonOptions);
    }

    public async Task<ParserStudent?> GetInfoAsync(string email)
    {
        var query = new Dictionary<string, string?>
        {
            ["email"] = email,
        };

        return await GetAsync<ParserStudent>("/student", query);
    }
}