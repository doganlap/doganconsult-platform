using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DoganConsult.Document.Data;
using Volo.Abp.DependencyInjection;

namespace DoganConsult.Document.EntityFrameworkCore;

public class EntityFrameworkCoreDocumentDbSchemaMigrator
    : IDocumentDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreDocumentDbSchemaMigrator(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolve the DocumentDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<DocumentDbContext>()
            .Database
            .MigrateAsync();
    }
}
