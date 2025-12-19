using System.Collections.Generic;
using Riok.Mapperly.Abstractions;
using DoganConsult.Organization.Organizations;

namespace DoganConsult.Organization;

[Mapper]
public partial class OrganizationApplicationMappers
{
    [MapProperty(nameof(Organizations.Organization.Type), nameof(OrganizationDto.Type))]
    [MapperIgnoreSource(nameof(Organizations.Organization.ExtraProperties))]
    [MapperIgnoreSource(nameof(Organizations.Organization.ConcurrencyStamp))]
    public partial OrganizationDto ToOrganizationDto(Organizations.Organization source);
    
    public partial List<OrganizationDto> ToOrganizationDtoList(List<Organizations.Organization> source);
}
