using System.Collections.Generic;
using System.Linq;

namespace Core.Models.Dto;

public partial class DocumentDto : IMappable<DocumentDto, Document>
{
    public static DocumentDto Map(Document orig)
    {
        return new DocumentDto()
        {
            Id = orig.Id,
            Name = orig.Name,
            MimeType = orig.MimeType,
        };
    }

    public static List<DocumentDto> Maps(List<Document> origs)
    {
        return origs.Select(o => DocumentDto.Map(o)).ToList();
    }
}
