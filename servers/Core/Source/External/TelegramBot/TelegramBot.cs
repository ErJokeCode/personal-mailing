using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace Core.External.TelegramBot;

public class TelegramBot : ITelegramBot
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly string _telegramBotUrl;

    public TelegramBot(IOptions<TelegarmBotOptions> options, IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
        _telegramBotUrl = options.Value.TelegarmBotUrl;
    }

    public async Task<bool> SendNotificationAsync(string chatId, string text)
    {
        var client = _clientFactory.CreateClient();
        client.BaseAddress = new Uri(_telegramBotUrl);

        var message = new
        {
            chat_id = chatId,
            text = text,
        };

        var uri = QueryHelpers.AddQueryString($"/send/{chatId}", "text", text);
        var response = await client.PostAsJsonAsync(uri, message);

        return response.IsSuccessStatusCode;
    }
}