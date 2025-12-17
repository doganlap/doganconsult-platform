using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace DoganConsult.AI.Data;

/* This is used if database provider does't define
 * IAIDbSchemaMigrator implementation.
 */
public class NullAIDbSchemaMigrator : IAIDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
