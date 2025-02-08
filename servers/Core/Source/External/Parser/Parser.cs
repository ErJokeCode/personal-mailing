using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Models;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace Core.External.Parser;

public class Parser : IParser
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly string _parserUrl;

    public Parser(IOptions<ParserOptions> options)
    {
        _parserUrl = options.Value.ParserUrl;

        _client = new()
        {
            BaseAddress = new Uri(_parserUrl)
        };

        _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        };
    }

    public async Task<T?> GetAsync<T>(string path, Dictionary<string, string?> query)
    {
        var response = await _client.GetAsync(QueryHelpers.AddQueryString(path, query));

        if (!response.IsSuccessStatusCode)
        {
            return default;
        }

        var result = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(result, _jsonOptions);
    }

    public async Task<bool> IncludeInfoAsync(Student student)
    {
        var query = new Dictionary<string, string?>
        {
            ["email"] = student.Email,
        };

        var info = await GetAsync<ParserStudent>("/student", query);

        if (info == null)
        {
            return false;
        }
        else
        {
            student.Info = info;
            return true;
        }
    }

    public async Task<bool> IncludeInfoAsync(IEnumerable<Student> students)
    {
        var anyFalse = false;

        foreach (var student in students)
        {
            var result = await IncludeInfoAsync(student);

            if (result == false)
            {
                anyFalse = true;
            }
        }

        return anyFalse;
    }
}