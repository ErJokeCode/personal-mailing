using System.Collections.Generic;
using Notify.Models;
using Notify.Routes.Documents.DTOs;
using Riok.Mapperly.Abstractions;
using Shared.Models;

namespace Notify.Routes.Documents.Maps;

[Mapper]
public partial class DocumentMapper
{
    public partial DocumentDto Map(Document document);
    public partial IEnumerable<DocumentDto> Map(IEnumerable<Document> document);
}