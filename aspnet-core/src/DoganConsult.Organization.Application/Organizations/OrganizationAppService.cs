using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoganConsult.Organization;
using DoganConsult.Organization.Organizations;
using DoganConsult.Organization.Domain.Shared.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace DoganConsult.Organization.Organizations;

[Authorize]
public class OrganizationAppService : ApplicationService, IOrganizationAppService
{
    private readonly IRepository<Organization, Guid> _organizationRepository;
    private readonly OrganizationApplicationMappers _mapper;

    public OrganizationAppService(
        IRepository<Organization, Guid> organizationRepository,
        OrganizationApplicationMappers mapper)
    {
        _organizationRepository = organizationRepository;
        _mapper = mapper;
    }

    [Authorize(OrganizationPermissions.Content.DocumentsCreate)] // Example of new permission
    public async Task<OrganizationDto> CreateAsync(CreateUpdateOrganizationDto input)
    {
        var organization = new Organization(
            GuidGenerator.Create(),
            input.Code,
            input.Name,
            (OrganizationType)input.Type,
            CurrentTenant.Id
        )
        {
            LegalName = input.LegalName,
            Sector = input.Sector,
            Country = input.Country,
            City = input.City,
            Regulators = input.Regulators,
            Status = input.Status,
            PrimaryContactEmail = input.PrimaryContactEmail,
            Notes = input.Notes
        };

        await _organizationRepository.InsertAsync(organization);

        return _mapper.ToOrganizationDto(organization);
    }

    [Authorize(OrganizationPermissions.Org.Organizations)]
    public async Task<OrganizationDto> GetAsync(Guid id)
    {
        var organization = await _organizationRepository.GetAsync(id);
        return _mapper.ToOrganizationDto(organization);
    }

    [Authorize(OrganizationPermissions.Org.Organizations)]
    public async Task<PagedResultDto<OrganizationDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        var queryable = await _organizationRepository.GetQueryableAsync();
        var query = queryable
            .OrderBy(x => x.Name)
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount);
        var organizations = await AsyncExecuter.ToListAsync(query);

        var totalCount = await _organizationRepository.GetCountAsync();

        return new PagedResultDto<OrganizationDto>(
            totalCount,
            _mapper.ToOrganizationDtoList(organizations)
        );
    }

    [Authorize(OrganizationPermissions.Org.WorkspacesManage)] // Example of new permission
    public async Task<OrganizationDto> UpdateAsync(Guid id, CreateUpdateOrganizationDto input)
    {
        var organization = await _organizationRepository.GetAsync(id);

        organization.Code = input.Code;
        organization.Name = input.Name;
        organization.LegalName = input.LegalName;
        organization.Type = (OrganizationType)input.Type;
        organization.Sector = input.Sector;
        organization.Country = input.Country;
        organization.City = input.City;
        organization.Regulators = input.Regulators;
        organization.Status = input.Status;
        organization.PrimaryContactEmail = input.PrimaryContactEmail;
        organization.Notes = input.Notes;

        await _organizationRepository.UpdateAsync(organization);

        return _mapper.ToOrganizationDto(organization);
    }

    [Authorize(OrganizationPermissions.Content.DocumentsDelete)] // Example of new permission
    public async Task DeleteAsync(Guid id)
    {
        await _organizationRepository.DeleteAsync(id);
    }

    [Authorize(OrganizationPermissions.Org.Organizations)]
    public async Task<long> GetCountAsync()
    {
        return await _organizationRepository.GetCountAsync();
    }

    [Authorize(OrganizationPermissions.Platform.Dashboard)] // Example of new permission
    public async Task<OrganizationStatisticsDto> GetStatisticsAsync()
    {
        var queryable = await _organizationRepository.GetQueryableAsync();
        var allOrgs = await AsyncExecuter.ToListAsync(queryable);
        
        var statistics = new OrganizationStatisticsDto
        {
            TotalOrganizations = allOrgs.Count,
            ActiveOrganizations = allOrgs.Count(o => o.Status == "active"),
            InactiveOrganizations = allOrgs.Count(o => o.Status != "active")
        };

        // Group by type
        statistics.ByType = allOrgs
            .GroupBy(o => o.Type.ToString())
            .ToDictionary(g => g.Key, g => g.Count());

        // Group by sector
        statistics.BySector = allOrgs
            .Where(o => !string.IsNullOrEmpty(o.Sector))
            .GroupBy(o => o.Sector!)
            .ToDictionary(g => g.Key, g => g.Count());

        // Group by country
        statistics.ByCountry = allOrgs
            .Where(o => !string.IsNullOrEmpty(o.Country))
            .GroupBy(o => o.Country!)
            .ToDictionary(g => g.Key, g => g.Count());

        // Trends by month (last 6 months)
        var sixMonthsAgo = DateTime.UtcNow.AddMonths(-6);
        statistics.Trends = allOrgs
            .Where(o => o.CreationTime >= sixMonthsAgo)
            .GroupBy(o => new { o.CreationTime.Year, o.CreationTime.Month })
            .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
            .Select(g => new TrendDataPoint
            {
                Label = $"{g.Key.Year}-{g.Key.Month:D2}",
                Count = g.Count(),
                Date = new DateTime(g.Key.Year, g.Key.Month, 1)
            })
            .ToList();

        return statistics;
    }
}
