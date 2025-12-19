using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace DoganConsult.Organization.Organizations;

public interface IOrganizationAppService : ICrudAppService<
    OrganizationDto,
    Guid,
    PagedAndSortedResultRequestDto,
    CreateUpdateOrganizationDto,
    CreateUpdateOrganizationDto>
{
    Task<long> GetCountAsync();    Task<OrganizationStatisticsDto> GetStatisticsAsync();}
