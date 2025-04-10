using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Chatter.Abstractions.MailService;
using Chatter.Models;
using Shared.Models;
using Shared.Services.FileStorage;

namespace Chatter.Infrastructure.Services;

public class TelegramBot : IMailService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly string _telegramBotUrl;
    private readonly IFileStorage _fileStorage;

    public TelegramBot(IOptions<MailServiceOptions> options, IHttpClientFactory clientFactory, IFileStorage fileStorage)
    {
        _clientFactory = clientFactory;
        _telegramBotUrl = options.Value.MailServiceUrl;
        _fileStorage = fileStorage;
    }

    private async Task<bool> SendAsync(string id, string content, IEnumerable<Document>? documents = null, bool isNotification = true)
    {
        var client = _clientFactory.CreateClient();
        client.BaseAddress = new Uri(_telegramBotUrl);

        if (documents is null || !documents.Any())
        {
            var message = new
            {
                chat_id = id,
                text = content,
            };

            string uri;

            if (isNotification)
            {
                uri = QueryHelpers.AddQueryString($"/send/{id}", "text", content);
            }
            else
            {
                uri = QueryHelpers.AddQueryString($"/chat_student/{id}/text", "text", content);
            }

            try
            {
                var response = await client.PostAsJsonAsync(uri, message);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
        else
        {
            var formData = new MultipartFormDataContent
            {
                { new StringContent(id), "chat_ids" }
            };

            foreach (var document in documents)
            {
                var blobData = await _fileStorage.DownloadAsync(document.BlobId);

                if (blobData is not null)
                {
                    formData.Add(new StreamContent(blobData.Stream), "files", document.Name);
                }
            }

            string uri;

            if (isNotification)
            {
                uri = QueryHelpers.AddQueryString($"/send/files", "text", content);
            }
            else
            {
                uri = QueryHelpers.AddQueryString($"/chat_student/{id}/files", "text", content);
            }

            try
            {
                var response = await client.PostAsync(uri, formData);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }

    public async Task<bool> SendNotificationAsync(string id, string content, IEnumerable<Document>? documents = null)
    {
        return await SendAsync(id, content, documents);
    }

    public async Task<bool> SendMessageAsync(string id, string content, IEnumerable<Document>? documents = null)
    {
        return await SendAsync(id, content, documents, false);
    }
}