# Health Checks Implementation Guide

## Status: Organization Service Complete ✅

Health checks have been implemented for the Organization service as a template. The same pattern needs to be applied to all other services.

## Implementation Pattern

### 1. Add NuGet Packages
Add to each service's `.csproj` file:
```xml
<PackageReference Include="AspNetCore.HealthChecks.Npgsql" Version="8.0.1" />
<PackageReference Include="AspNetCore.HealthChecks.Redis" Version="8.0.1" />
```

### 2. Create Health Check Class
Create `HealthChecks/{ServiceName}HealthCheck.cs` in each HttpApi.Host project:
```csharp
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
// Import your main entity

public class {ServiceName}HealthCheck : IHealthCheck
{
    private readonly IRepository<{Entity}, Guid> _repository;

    public {ServiceName}HealthCheck(IRepository<{Entity}, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _repository.GetCountAsync(cancellationToken);
            return HealthCheckResult.Healthy("{ServiceName} service is healthy");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("{ServiceName} service is unhealthy", ex);
        }
    }
}
```

### 3. Update HttpApiHostModule
Add to `ConfigureServices`:
```csharp
ConfigureHealthChecks(context, configuration);
```

Add method:
```csharp
private void ConfigureHealthChecks(ServiceConfigurationContext context, IConfiguration configuration)
{
    var healthChecksBuilder = context.Services.AddHealthChecks();
    
    var connectionString = configuration.GetConnectionString("Default");
    if (!string.IsNullOrEmpty(connectionString))
    {
        healthChecksBuilder.AddNpgSql(connectionString, name: "postgres", tags: new[] { "db", "sql", "postgresql" });
    }

    var redisConfiguration = configuration["Redis:Configuration"];
    if (!string.IsNullOrEmpty(redisConfiguration))
    {
        healthChecksBuilder.AddRedis(redisConfiguration, name: "redis", tags: new[] { "cache", "redis" });
    }

    healthChecksBuilder.AddCheck<{ServiceName}HealthCheck>("{servicename}", tags: new[] { "app", "{servicename}" });
}
```

Add to `OnApplicationInitialization`:
```csharp
app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = System.Text.Json.JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description,
                exception = e.Value.Exception?.Message,
                duration = e.Value.Duration.ToString()
            })
        });
        await context.Response.WriteAsync(result);
    }
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("db") || check.Tags.Contains("cache")
});

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false
});
```

## Services to Update

- [x] Organization ✅
- [ ] Workspace
- [ ] Document
- [ ] UserProfile
- [ ] AI
- [ ] Audit
- [ ] Identity
- [ ] Gateway

## Testing

After implementation, test each service:
```bash
curl https://localhost:44337/health
curl https://localhost:44337/health/ready
curl https://localhost:44337/health/live
```

Expected response:
```json
{
  "status": "Healthy",
  "checks": [
    {
      "name": "postgres",
      "status": "Healthy",
      "description": null,
      "exception": null,
      "duration": "00:00:00.0123456"
    },
    {
      "name": "redis",
      "status": "Healthy",
      "description": null,
      "exception": null,
      "duration": "00:00:00.0012345"
    },
    {
      "name": "organization",
      "status": "Healthy",
      "description": "Organization service is healthy",
      "exception": null,
      "duration": "00:00:00.0023456"
    }
  ]
}
```
