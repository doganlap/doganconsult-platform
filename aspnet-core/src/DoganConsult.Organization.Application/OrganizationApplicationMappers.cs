using System.Collections.Generic;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;
using DoganConsult.Organization.Organizations;

namespace DoganConsult.Organization;

[Mapper]
public partial class OrganizationApplicationMappers
{
    [MapProperty(nameof(Organizations.Organization.Type), nameof(OrganizationDto.Type))]
    public partial OrganizationDto Map(Organizations.Organization source);
}
