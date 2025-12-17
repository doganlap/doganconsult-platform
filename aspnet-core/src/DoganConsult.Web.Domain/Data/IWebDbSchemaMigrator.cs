using System.Threading.Tasks;

namespace DoganConsult.Web.Data;

public interface IWebDbSchemaMigrator
{
    Task MigrateAsync();
}
