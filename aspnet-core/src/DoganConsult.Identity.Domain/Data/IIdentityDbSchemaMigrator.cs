using System.Threading.Tasks;

namespace DoganConsult.Identity.Data;

public interface IIdentityDbSchemaMigrator
{
    Task MigrateAsync();
}
