using System.Threading.Tasks;

namespace DoganConsult.Document.Data;

public interface IDocumentDbSchemaMigrator
{
    Task MigrateAsync();
}
