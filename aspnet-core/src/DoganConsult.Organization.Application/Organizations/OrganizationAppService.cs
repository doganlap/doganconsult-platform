using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoganConsult.Organization.Organizations;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace DoganConsult.Organization.Organizations;

[Authorize]
public class OrganizationAppService : ApplicationService, IOrganizationAppService
{
    private readonly IRepository<Organization, Guid> _organizationRepository;

    public OrganizationAppService(IRepository<Organization, Guid> organizationRepository)
    {
        _organizationRepository = organizationRepository;
    }

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
            Sector = input.Sector,
            Country = input.Country,
            City = input.City,
            Regulators = input.Regulators,
            Status = input.Status,
            PrimaryContactEmail = input.PrimaryContactEmail,
            Notes = input.Notes
        };

        await _organizationRepository.InsertAsync(organization);

        return ObjectMapper.Map<Organization, OrganizationDto>(organization);
    }

    public async Task<OrganizationDto> GetAsync(Guid id)
    {
        var organization = await _organizationRepository.GetAsync(id);
        return ObjectMapper.Map<Organization, OrganizationDto>(organization);
    }

    public async Task<PagedResultDto<OrganizationDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        var queryable = await _organizationRepository.GetQueryableAsync();
        var organizations = queryable
            .OrderBy(x => x.Name)
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount)
            .ToList();

        var totalCount = await _organizationRepository.GetCountAsync();

        return new PagedResultDto<OrganizationDto>(
            totalCount,
            ObjectMapper.Map<List<Organization>, List<OrganizationDto>>(organizations)
        );
    }

    public async Task<OrganizationDto> UpdateAsync(Guid id, CreateUpdateOrganizationDto input)
    {
        var organization = await _organizationRepository.GetAsync(id);

        organization.Code = input.Code;
        organization.Name = input.Name;
        organization.Type = (OrganizationType)input.Type;
        organization.Sector = input.Sector;
        organization.Country = input.Country;
        organization.City = input.City;
        organization.Regulators = input.Regulators;
        organization.Status = input.Status;
        organization.PrimaryContactEmail = input.PrimaryContactEmail;
        organization.Notes = input.Notes;

        await _organizationRepository.UpdateAsync(organization);

        return ObjectMapper.Map<Organization, OrganizationDto>(organization);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _organizationRepository.DeleteAsync(id);
    }
}
