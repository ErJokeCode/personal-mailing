using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Core.Abstractions;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace Core.Infrastructure.Services;

public class TelegramBot : IMailService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly string _telegramBotUrl;

    public TelegramBot(IOptions<MailServiceOptions> options, IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
        _telegramBotUrl = options.Value.MailServiceUrl;
    }

    public async Task<bool> SendNotificationAsync(string id, string content)
    {
        var client = _clientFactory.CreateClient();
        client.BaseAddress = new Uri(_telegramBotUrl);

        var message = new
        {
            chat_id = id,
            text = content,
        };

        var uri = QueryHelpers.AddQueryString($"/send/{id}", "text", content);
        var response = await client.PostAsJsonAsync(uri, message);

        return response.IsSuccessStatusCode;
    }
}