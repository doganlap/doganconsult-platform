using Xunit;

namespace DoganConsult.Document.EntityFrameworkCore;

[CollectionDefinition(DocumentTestConsts.CollectionDefinitionName)]
public class DocumentEntityFrameworkCoreCollection : ICollectionFixture<DocumentEntityFrameworkCoreFixture>
{

}
