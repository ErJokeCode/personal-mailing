using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Models;

namespace Core.External.Parser;

public class ParserOptions
{
    public required string ParserUrl { get; set; }
}

public interface IParser
{
    public Task<T?> GetAsync<T>(string path, Dictionary<string, string?> query);
    public Task<bool> IncludeInfoAsync(Student student);
    public Task<bool> IncludeInfoAsync(IEnumerable<Student> students);
}