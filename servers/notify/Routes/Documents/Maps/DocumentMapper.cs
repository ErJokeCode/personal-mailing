using System.Collections.Generic;
using Core.Routes.Documents.DTOs;
using Notify.Models;
using Riok.Mapperly.Abstractions;

namespace Core.Routes.Documents.Maps;

[Mapper]
public partial class DocumentMapper
{
    public partial DocumentDto Map(Document document);
    public partial IEnumerable<DocumentDto> Map(IEnumerable<Document> document);
}