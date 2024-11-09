using System.IO;
using System.Threading.Tasks;
using Core.Models.Dto;
using Core.Utility;
using Microsoft.AspNetCore.Http;

namespace Core.Handlers;

public static partial class DocumentHandler
{
    public static async Task<IResult> GetDocument(int id, CoreDb db)
    {
        var document = await db.Documents.FindAsync(id);

        if (document == null)
        {
            return Results.NotFound("Could not find the document");
        }

        var dto = DocumentDto.Map(document);

        return Results.Ok(dto);
    }

    public static async Task<IResult> GetDocumentData(int id, CoreDb db)
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
