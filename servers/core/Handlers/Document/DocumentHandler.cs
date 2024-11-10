using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Core.Models;
using Core.Models.Dto;
using Core.Utility;
using Microsoft.AspNetCore.Http;

namespace Core.Handlers;

public static partial class DocumentHandler
{
    public static async Task StoreDocuments(IFormFileCollection documents, int id, CoreDb db,
                                            bool isNotification = true)
    {
        foreach (var document in documents)
        {
            var mimeType = MimeTypes.GetMimeType(document.FileName);

            var newDocument = new Document()
            {
                Name = document.FileName,
                MimeType = mimeType,
                InternalName = Guid.NewGuid().ToString(),
            };

            if (isNotification)
            {
                newDocument.NotificationId = id;
            }
            else
            {
                newDocument.MessageId = id;
            }

            var path = Path.Combine(Directory.GetCurrentDirectory(), "Documents", newDocument.InternalName);
            var writeFile = File.Create(path);
            var stream = document.OpenReadStream();
            stream.Position = 0;
            await stream.CopyToAsync(writeFile);

            db.Documents.Add(newDocument);
        }

        await db.SaveChangesAsync();
    }
}
