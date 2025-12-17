using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace DoganConsult.UserProfile.Data;

/* This is used if database provider does't define
 * IUserProfileDbSchemaMigrator implementation.
 */
public class NullUserProfileDbSchemaMigrator : IUserProfileDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
