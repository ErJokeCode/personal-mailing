using System.Threading.Tasks;
using Core.Models;
using Core.Utility;
using Microsoft.AspNetCore.Http;

namespace Core.Handlers;

public static partial class DataHandler
{
    public class TextDetails
    {
        public string Content { get; set; }
    }

    public static async Task<IResult> SaveText(TextDetails details, CoreDb db)
    {
        var text = new Text()
        {
            Content = details.Content,
        };

        db.Texts.Add(text);
        await db.SaveChangesAsync();

        return Results.Created("", text);
    }
}
