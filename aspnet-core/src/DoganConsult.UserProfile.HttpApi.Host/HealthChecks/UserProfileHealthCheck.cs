using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using DoganConsult.UserProfile.UserProfiles;

namespace DoganConsult.UserProfile.HealthChecks;

public class UserProfileHealthCheck : IHealthCheck
{
    private readonly IRepository<UserProfiles.UserProfile, Guid> _userProfileRepository;

    public UserProfileHealthCheck(IRepository<UserProfiles.UserProfile, Guid> userProfileRepository)
    {
        _userProfileRepository = userProfileRepository;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _userProfileRepository.GetCountAsync(cancellationToken);
            return HealthCheckResult.Healthy("UserProfile service is healthy");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("UserProfile service is unhealthy", ex);
        }
    }
}
