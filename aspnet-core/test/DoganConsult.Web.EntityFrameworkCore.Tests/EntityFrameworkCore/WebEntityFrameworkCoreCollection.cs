using Xunit;

namespace DoganConsult.Web.EntityFrameworkCore;

[CollectionDefinition(WebTestConsts.CollectionDefinitionName)]
public class WebEntityFrameworkCoreCollection : ICollectionFixture<WebEntityFrameworkCoreFixture>
{

}
