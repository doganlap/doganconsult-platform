using System.Threading.Tasks;

namespace DoganConsult.AI.Data;

public interface IAIDbSchemaMigrator
{
    Task MigrateAsync();
}
