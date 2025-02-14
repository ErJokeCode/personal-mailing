using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Core.Abstractions;
using Core.Abstractions.FileStorage;
using Core.Abstractions.MailService;
using Core.Models;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace Core.Infrastructure.Services;

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

    public async Task<bool> SendNotificationAsync(string id, string content, IEnumerable<Document>? documents = null)
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

            var uri = QueryHelpers.AddQueryString($"/send/{id}", "text", content);

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

            var uri = QueryHelpers.AddQueryString($"/send/files", "text", content);

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
}