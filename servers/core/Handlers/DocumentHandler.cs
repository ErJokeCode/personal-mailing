using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Core.Handlers;

public static class DocumentHandler
{
    public static async Task<List<string>> StoreDocuments(IFormFileCollection documents)
    {
        var now = DateTime.Now.ToString();
        var fileNames = new List<string>();

        foreach (var document in documents)
        {
            var fileName = (now + " " + document.FileName).Replace("/", "-");
            fileNames.Add(fileName);

            var path = Path.Combine(Directory.GetCurrentDirectory(), "Documents", fileName);
            var writeFile = File.Create(path);
            var stream = document.OpenReadStream();
            stream.Position = 0;
            await stream.CopyToAsync(writeFile);
        }

        return fileNames;
    }

    public static async Task<IResult> GetDocument(string name)
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "Documents", name);
        var mimeType = MimeTypes.GetMimeType(name);

        return Results.File(path, mimeType);
    }
}
