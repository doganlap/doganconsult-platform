using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace DoganConsult.Web.Data;

/* This is used if database provider does't define
 * IWebDbSchemaMigrator implementation.
 */
public class NullWebDbSchemaMigrator : IWebDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
