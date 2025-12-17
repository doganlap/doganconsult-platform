using System.Threading.Tasks;

namespace DoganConsult.UserProfile.Data;

public interface IUserProfileDbSchemaMigrator
{
    Task MigrateAsync();
}
