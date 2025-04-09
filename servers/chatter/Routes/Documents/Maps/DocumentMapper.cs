using System.Collections.Generic;
using Chatter.Models;
using Chatter.Routes.Documents.DTOs;
using Riok.Mapperly.Abstractions;
using Shared.Models;

namespace Chatter.Routes.Documents.Maps;

[Mapper]
public partial class DocumentMapper
{
    public partial DocumentDto Map(Document document);
    public partial IEnumerable<DocumentDto> Map(IEnumerable<Document> document);
}