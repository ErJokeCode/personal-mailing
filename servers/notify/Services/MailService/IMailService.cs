using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Notify.Models;
using Shared.Models;

namespace Notify.Services.MailService;

public class MailServiceOptions
{
    public required string MailServiceUrl { get; set; }
}

public interface IMailService
{
    public Task<bool> SendNotificationAsync(string id, string content, IEnumerable<Document>? documents = null);
    public Task<bool> SendMessageAsync(string id, string content, IEnumerable<Document>? documents = null);
}