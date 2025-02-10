using System.Threading.Tasks;

namespace Core.External.TelegramBot;

public class TelegarmBotOptions
{
    public required string TelegarmBotUrl { get; set; }
}

public interface ITelegramBot
{
    public Task<bool> SendNotificationAsync(string chatId, string text);
}