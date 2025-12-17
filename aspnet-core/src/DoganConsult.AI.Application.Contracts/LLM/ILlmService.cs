using System.Threading.Tasks;

namespace DoganConsult.AI.LLM;

public interface ILlmService
{
    Task<string> SummarizeAsync(string text, string organizationCode, string? modelName = null);
    Task<string> AnalyzeAsync(string text, string? modelName = null);
}
