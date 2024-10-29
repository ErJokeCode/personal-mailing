using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Core.Models;
using Core.Utility;
using Microsoft.AspNetCore.Http;

namespace Core.Handlers;

public static class DocumentHandler
{
    public static async Task StoreDocuments(IFormFileCollection documents, int notificationId, CoreDb db)
    {
        foreach (var document in documents)
        {
            var mimeType = MimeTypes.GetMimeType(document.FileName);

            var newDocument = new Document() {
                Name = document.FileName,
                MimeType = mimeType,
                NotificationId = notificationId,
                InternalName = Guid.NewGuid().ToString(),
            };

            var path = Path.Combine(Directory.GetCurrentDirectory(), "Documents", newDocument.InternalName);
            var writeFile = File.Create(path);
            var stream = document.OpenReadStream();
            stream.Position = 0;
            await stream.CopyToAsync(writeFile);

            db.Documents.Add(newDocument);
        }

        await db.SaveChangesAsync();
    }

    public static async Task<IResult> GetDocument(int id, CoreDb db)
    {
        var document = await db.Documents.FindAsync(id);

        if (document == null)
        {
            return Results.NotFound("Could not find the document");
        }

        var path = Path.Combine(Directory.GetCurrentDirectory(), "Documents", document.InternalName);

        return Results.File(path, document.MimeType, document.Name);
    }
}
