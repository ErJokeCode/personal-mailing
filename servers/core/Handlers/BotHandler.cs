using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;

namespace Core.Handlers;

public static class BotHandler
{
    public class BotMessage
    {
        public string chat_id { get; set; }
        public string text { get; set; }
    }

    public static async Task<bool> SendToBot(string chatId, string text, IFormFileCollection documents,
                                             bool isNotification = true)
    {
        HttpClient httpClient = new() { BaseAddress = new Uri("http://bot:8000/") };

        if (documents != null && documents.Count > 0)
        {
            var content = new MultipartFormDataContent();

            content.Add(new StringContent(chatId), "chat_ids");

            foreach (var document in documents)
            {
                content.Add(new StreamContent(document.OpenReadStream()), "files", document.FileName);
            }

            if (isNotification)
            {
                var uri = QueryHelpers.AddQueryString($"/send/files", "text", text);
                var response = await httpClient.PostAsync(uri, content);
                return response.IsSuccessStatusCode;
            }
            else
            {
                var uri = QueryHelpers.AddQueryString($"/chat_student/{chatId}/files", "text", text);
                var response = await httpClient.PostAsync(uri, content);
                return response.IsSuccessStatusCode;
            }
        }
        else
        {
            var message = new BotMessage()
            {
                chat_id = chatId,
                text = text,
            };

            if (isNotification)
            {
                var uri = QueryHelpers.AddQueryString($"/send/{chatId}", "text", text);
                var response = await httpClient.PostAsJsonAsync(uri, message);
                return response.IsSuccessStatusCode;
            }
            else
            {
                var uri = QueryHelpers.AddQueryString($"/chat_student/{chatId}/text", "text", text);
                var response = await httpClient.PostAsJsonAsync(uri, message);
                return response.IsSuccessStatusCode;
            }
        }
    }
}
