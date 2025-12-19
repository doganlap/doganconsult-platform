using System.Collections.Generic;
using DoganConsult.Document.Documents;
using Riok.Mapperly.Abstractions;

namespace DoganConsult.Document.Documents;

[Mapper]
public partial class DocumentObjectMapper
{
    [MapperIgnoreSource(nameof(Document.ExtraProperties))]
    [MapperIgnoreSource(nameof(Document.ConcurrencyStamp))]
    public partial DocumentDto ToDocumentDto(Documents.Document document);
    public partial List<DocumentDto> ToDocumentDtos(List<Documents.Document> documents);
}
