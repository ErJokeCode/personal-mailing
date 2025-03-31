using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Models;

namespace Core.Abstractions.MailService;

public class MailServiceOptions
{
    public required string MailServiceUrl { get; set; }
}

public interface IMailService
{
    public Task<bool> SendNotificationAsync(string id, string content, IEnumerable<Document>? documents = null);
    public Task<bool> SendMessageAsync(string id, string content, IEnumerable<Document>? documents = null);
}