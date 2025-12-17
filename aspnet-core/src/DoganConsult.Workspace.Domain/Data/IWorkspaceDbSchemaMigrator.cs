using System.Threading.Tasks;

namespace DoganConsult.Workspace.Data;

public interface IWorkspaceDbSchemaMigrator
{
    Task MigrateAsync();
}
