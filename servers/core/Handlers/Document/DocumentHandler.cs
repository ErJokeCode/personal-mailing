using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Core.Utility;
using Microsoft.AspNetCore.Http;

namespace Core.Handlers;

public static partial class DocumentHandler
{
    public static ICollection<Document> GetFromIds(List<int> ids, CoreDb db)
    {
        var collection = new List<Document>();

        foreach (var id in ids)
        {
            var document = db.Documents.Find(id);

            if (document != null)
            {
                collection.Add(document);
            }
        }

        return collection;
    }

    public static async Task<List<int>> StoreDocuments(IFormFileCollection documents, CoreDb db)
    {
        var savedDocs = new List<Document>();

        foreach (var document in documents)
        {
            var mimeType = MimeTypes.GetMimeType(document.FileName);

            var newDocument = new Document()
            {
                Name = document.FileName,
                MimeType = mimeType,
                InternalName = Guid.NewGuid().ToString(),
            };

            var path = Path.Combine(Directory.GetCurrentDirectory(), "Documents", newDocument.InternalName);
            var writeFile = File.Create(path);
            var stream = document.OpenReadStream();
            stream.Position = 0;
            await stream.CopyToAsync(writeFile);

            db.Documents.Add(newDocument);
            savedDocs.Add(newDocument);
        }

        await db.SaveChangesAsync();

        return savedDocs.Select(d => d.Id).ToList();
    }

    public static async Task DeleteDocuments(List<int> ids, CoreDb db)
    {
        foreach (var id in ids)
        {
            var document = await db.Documents.FindAsync(id);

            var path = Path.Combine(Directory.GetCurrentDirectory(), "Documents", document.InternalName);
            File.Delete(path);

            db.Documents.Remove(document);
        }

        await db.SaveChangesAsync();
    }
}
