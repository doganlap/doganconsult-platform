using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DoganConsult.Organization.Data;
using Volo.Abp.DependencyInjection;

namespace DoganConsult.Organization.EntityFrameworkCore;

public class EntityFrameworkCoreOrganizationDbSchemaMigrator
    : IOrganizationDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreOrganizationDbSchemaMigrator(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolve the OrganizationDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<OrganizationDbContext>()
            .Database
            .MigrateAsync();
    }
}
