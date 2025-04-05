using System.Collections.Generic;
using Core.Models;
using Core.Routes.Documents.DTOs;
using Riok.Mapperly.Abstractions;
using Shared.Models;

namespace Core.Routes.Documents.Maps;

[Mapper]
public partial class DocumentMapper
{
    public partial DocumentDto Map(Document document);
    public partial IEnumerable<DocumentDto> Map(IEnumerable<Document> document);
}