using Core.External.TelegramBot;

namespace Core.Tests.Mocks;

public class TelegramBotMock : ITelegramBot
{
    public Task<bool> SendNotificationAsync(string chatId, string text)
    {
        return Task.FromResult(true);
    }
}