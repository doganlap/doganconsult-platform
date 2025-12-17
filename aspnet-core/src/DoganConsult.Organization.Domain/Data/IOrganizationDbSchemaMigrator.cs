using System.Threading.Tasks;

namespace DoganConsult.Organization.Data;

public interface IOrganizationDbSchemaMigrator
{
    Task MigrateAsync();
}
