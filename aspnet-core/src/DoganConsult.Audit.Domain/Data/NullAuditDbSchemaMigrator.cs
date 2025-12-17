using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace DoganConsult.Audit.Data;

/* This is used if database provider does't define
 * IAuditDbSchemaMigrator implementation.
 */
public class NullAuditDbSchemaMigrator : IAuditDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
