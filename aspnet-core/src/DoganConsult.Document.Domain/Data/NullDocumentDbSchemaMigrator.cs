using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace DoganConsult.Document.Data;

/* This is used if database provider does't define
 * IDocumentDbSchemaMigrator implementation.
 */
public class NullDocumentDbSchemaMigrator : IDocumentDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
