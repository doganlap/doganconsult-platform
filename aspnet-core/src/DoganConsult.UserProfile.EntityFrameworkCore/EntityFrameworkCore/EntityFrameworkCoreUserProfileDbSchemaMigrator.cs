using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DoganConsult.UserProfile.Data;
using Volo.Abp.DependencyInjection;

namespace DoganConsult.UserProfile.EntityFrameworkCore;

public class EntityFrameworkCoreUserProfileDbSchemaMigrator
    : IUserProfileDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreUserProfileDbSchemaMigrator(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolve the UserProfileDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<UserProfileDbContext>()
            .Database
            .MigrateAsync();
    }
}
