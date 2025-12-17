using DoganConsult.Document.Samples;
using Xunit;

namespace DoganConsult.Document.EntityFrameworkCore.Applications;

[Collection(DocumentTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<DocumentEntityFrameworkCoreTestModule>
{

}
