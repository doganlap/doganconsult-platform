using System.Collections.Generic;
using DoganConsult.Web.Demos;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace DoganConsult.Web;

[Mapper]
public partial class WebApplicationMappers
{
    /* You can configure your Mapperly mapping configuration here.
     * Alternatively, you can split your mapping configurations
     * into multiple mapper classes for a better organization. */

    // Demo Request Mappings
    [MapperIgnoreSource(nameof(DemoRequest.IsDeleted))]
    [MapperIgnoreSource(nameof(DemoRequest.DeleterId))]
    [MapperIgnoreSource(nameof(DemoRequest.DeletionTime))]
    [MapperIgnoreSource(nameof(DemoRequest.LastModificationTime))]
    [MapperIgnoreSource(nameof(DemoRequest.LastModifierId))]
    [MapperIgnoreSource(nameof(DemoRequest.CreatorId))]
    [MapperIgnoreSource(nameof(DemoRequest.ExtraProperties))]
    [MapperIgnoreSource(nameof(DemoRequest.ConcurrencyStamp))]
    public partial DemoRequestDto MapToDemoRequestDto(DemoRequest source);
    public partial List<DemoRequestDto> MapToDemoRequestDtoList(List<DemoRequest> source);
    
    [MapperIgnoreSource(nameof(DemoRequest.IsDeleted))]
    [MapperIgnoreSource(nameof(DemoRequest.DeleterId))]
    [MapperIgnoreSource(nameof(DemoRequest.DeletionTime))]
    [MapperIgnoreSource(nameof(DemoRequest.LastModificationTime))]
    [MapperIgnoreSource(nameof(DemoRequest.LastModifierId))]
    [MapperIgnoreSource(nameof(DemoRequest.CreatorId))]
    [MapperIgnoreSource(nameof(DemoRequest.ExtraProperties))]
    [MapperIgnoreSource(nameof(DemoRequest.ConcurrencyStamp))]
    [MapperIgnoreTarget(nameof(DemoRequestDetailDto.Activities))]
    public partial DemoRequestDetailDto MapToDemoRequestDetailDto(DemoRequest source);
}
