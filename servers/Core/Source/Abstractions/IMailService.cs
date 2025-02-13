using System.Threading.Tasks;

namespace Core.Abstractions;

public class MailServiceOptions
{
    public required string MailServiceUrl { get; set; }
}

public interface IMailService
{
    public Task<bool> SendNotificationAsync(string id, string content);
}