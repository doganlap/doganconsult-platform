using DoganConsult.Document.Samples;
using Xunit;

namespace DoganConsult.Document.EntityFrameworkCore.Domains;

[Collection(DocumentTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<DocumentEntityFrameworkCoreTestModule>
{

}
