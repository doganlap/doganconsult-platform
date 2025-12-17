using System;
using System.Threading.Tasks;

namespace DoganConsult.AI.AIRequests;

public interface ILlmService
{
    Task<string> SummarizeAsync(Guid organizationId, string text);
}
