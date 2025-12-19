using System.Collections.Generic;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;
using DoganConsult.AI.AIRequests;

namespace DoganConsult.AI;

[Mapper]
public partial class AIApplicationMappers
{
    [MapperIgnoreSource(nameof(AIRequest.ExtraProperties))]
    [MapperIgnoreSource(nameof(AIRequest.ConcurrencyStamp))]
    public partial AIRequestDto Map(AIRequest source);
    public partial List<AIRequestDto> Map(List<AIRequest> source);
}
