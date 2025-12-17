using System.Threading.Tasks;

namespace DoganConsult.Audit.Data;

public interface IAuditDbSchemaMigrator
{
    Task MigrateAsync();
}
