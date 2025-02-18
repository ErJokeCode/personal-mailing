using Core.Abstractions;

namespace Core.Tests.Mocks;

public class MailServiceMock : IMailService
{
    public Task<bool> SendNotificationAsync(string chatId, string text)
    {
        return Task.FromResult(true);
    }
}