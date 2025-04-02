using System.Collections.Generic;
using System.Threading.Tasks;

namespace Notify.Abstractions.Parser;

public class ParserOptions
{
    public required string ParserUrl { get; set; }
}

public interface IParser
{
    public Task<T?> GetAsync<T>(string path, Dictionary<string, string?> query);
    public Task<ParserStudent?> GetInfoAsync(string email);
}