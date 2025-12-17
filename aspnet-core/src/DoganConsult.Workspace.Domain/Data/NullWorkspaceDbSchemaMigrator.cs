using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace DoganConsult.Workspace.Data;

/* This is used if database provider does't define
 * IWorkspaceDbSchemaMigrator implementation.
 */
public class NullWorkspaceDbSchemaMigrator : IWorkspaceDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
