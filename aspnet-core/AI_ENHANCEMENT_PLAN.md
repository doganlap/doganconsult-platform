# AI Service Enhancement Plan - Microsoft Agent Framework Integration

## Current State Analysis
✅ **Already Implemented:**
- AI Service with basic LLM integration (hertze server)
- AI request logging to PostgreSQL
- Basic audit summary generation
- Redis configuration for caching

## 🚀 Enhancement Goals
1. **Replace basic LLM client with Microsoft Agent Framework**
2. **Add multi-model support (GitHub Models + existing hertze server)**
3. **Implement advanced AI features (multi-turn conversations, tool calling)**
4. **Add evaluation and monitoring capabilities**

## Implementation Strategy

### Step 1: Add Microsoft Agent Framework (.NET)

```bash
# Add to DoganConsult.AI.Infrastructure project
cd src/DoganConsult.AI.Infrastructure
dotnet add package Microsoft.Agents.AI.OpenAI --prerelease
dotnet add package Microsoft.Agents.AI.AzureAI --prerelease
dotnet add package OpenAI
```

### Step 2: Enhanced AI Service Interface

```csharp
// DoganConsult.AI.Application.Contracts/Services/IEnhancedAIService.cs
public interface IEnhancedAIService
{
    // Existing methods
    Task<string> GenerateAuditSummaryAsync(string auditData);
    
    // New enhanced methods
    Task<ChatResponse> ChatAsync(ChatRequest request);
    Task<IAsyncEnumerable<string>> ChatStreamingAsync(ChatRequest request);
    Task<DocumentAnalysisResult> AnalyzeDocumentAsync(DocumentAnalysisRequest request);
    Task<ComplianceCheckResult> CheckComplianceAsync(ComplianceCheckRequest request);
    Task<RiskAssessmentResult> AssessRiskAsync(RiskAssessmentRequest request);
    
    // Multi-turn conversation support
    Task<string> CreateConversationThreadAsync(Guid userId, Guid? tenantId = null);
    Task<ChatResponse> ContinueConversationAsync(string threadId, string message);
    
    // Tool calling capabilities
    Task<ToolCallResult> ExecuteToolAsync(string toolName, Dictionary<string, object> parameters);
}
```

### Step 3: Agent Configuration

```csharp
// DoganConsult.AI.Infrastructure/Configuration/AgentConfiguration.cs
public class AgentConfiguration
{
    public bool UseGitHubModels { get; set; } = true;
    public bool UseHertzeServer { get; set; } = true;
    
    // GitHub Models config
    public string GitHubToken { get; set; } = "";
    public string DefaultGitHubModel { get; set; } = "openai/gpt-4.1";
    
    // Hertze server config (existing)
    public string LlmServerEndpoint { get; set; } = "";
    public string ApiKey { get; set; } = "";
    
    // Agent settings
    public string DefaultSystemPrompt { get; set; } = "You are an expert consulting AI assistant specializing in audit, compliance, and risk assessment.";
    public int MaxTokens { get; set; } = 4000;
    public decimal Temperature { get; set; } = 0.7m;
}
```

### Step 4: Enhanced Implementation

```csharp
// DoganConsult.AI.Infrastructure/Services/EnhancedAIService.cs
public class EnhancedAIService : IEnhancedAIService, ITransientDependency
{
    private readonly IAIAgent _consultingAgent;
    private readonly IAIAgent _auditAgent;
    private readonly IAIAgent _complianceAgent;
    private readonly IRepository<AIRequest> _aiRequestRepository;
    private readonly IDistributedCache _cache;
    
    public EnhancedAIService(
        [KeyedService("consulting")] IAIAgent consultingAgent,
        [KeyedService("audit")] IAIAgent auditAgent,
        [KeyedService("compliance")] IAIAgent complianceAgent,
        IRepository<AIRequest> aiRequestRepository,
        IDistributedCache cache)
    {
        _consultingAgent = consultingAgent;
        _auditAgent = auditAgent;
        _complianceAgent = complianceAgent;
        _aiRequestRepository = aiRequestRepository;
        _cache = cache;
    }

    public async Task<DocumentAnalysisResult> AnalyzeDocumentAsync(DocumentAnalysisRequest request)
    {
        var startTime = DateTime.UtcNow;
        
        try
        {
            // Create specialized prompt for document analysis
            var analysisPrompt = $@"
            Analyze the following document for:
            1. Key insights and findings
            2. Compliance indicators
            3. Risk factors
            4. Recommendations
            
            Document Type: {request.DocumentType}
            Content: {request.Content}
            
            Provide structured analysis in JSON format.";

            var response = await _consultingAgent.RunAsync(analysisPrompt);
            
            // Log AI request
            await LogAIRequestAsync("DocumentAnalysis", analysisPrompt, response, startTime);
            
            return JsonSerializer.Deserialize<DocumentAnalysisResult>(response);
        }
        catch (Exception ex)
        {
            await LogAIRequestAsync("DocumentAnalysis", request.Content, $"Error: {ex.Message}", startTime);
            throw;
        }
    }

    public async Task<ComplianceCheckResult> CheckComplianceAsync(ComplianceCheckRequest request)
    {
        // Use specialized compliance agent
        var compliancePrompt = $@"
        Perform compliance check for:
        Organization: {request.OrganizationType}
        Sector: {request.Sector}
        Regulations: {string.Join(", ", request.ApplicableRegulations)}
        
        Data to check: {request.DataToCheck}
        
        Provide compliance assessment with:
        1. Compliance status (Compliant/Non-Compliant/Needs Review)
        2. Specific violations found
        3. Recommendations for remediation
        4. Risk level (Low/Medium/High/Critical)";

        var response = await _complianceAgent.RunAsync(compliancePrompt);
        return JsonSerializer.Deserialize<ComplianceCheckResult>(response);
    }
}
```

### Step 5: Agent Factory with Multi-Model Support

```csharp
// DoganConsult.AI.Infrastructure/Factories/AIAgentFactory.cs
public class AIAgentFactory
{
    private readonly AgentConfiguration _config;
    
    public IAIAgent CreateConsultingAgent()
    {
        if (_config.UseGitHubModels)
        {
            return CreateGitHubAgent("consulting");
        }
        else
        {
            return CreateHertzeAgent("consulting");
        }
    }
    
    private IAIAgent CreateGitHubAgent(string agentType)
    {
        var client = new OpenAIClient(
            new ApiKeyCredential(_config.GitHubToken),
            new OpenAIClientOptions
            {
                Endpoint = new Uri("https://models.github.ai/inference")
            }
        );

        var systemPrompt = agentType switch
        {
            "consulting" => "You are an expert management consultant specializing in organizational efficiency and strategic planning.",
            "audit" => "You are an expert auditor with deep knowledge of audit procedures, risk assessment, and compliance frameworks.",
            "compliance" => "You are a compliance expert specializing in regulatory frameworks, risk management, and legal requirements.",
            _ => _config.DefaultSystemPrompt
        };

        return client
            .GetChatClient(_config.DefaultGitHubModel)
            .CreateAIAgent(
                systemPrompt,
                $"DoganConsult{agentType}Agent",
                tools: CreateToolsForAgent(agentType)
            );
    }

    private List<AITool> CreateToolsForAgent(string agentType)
    {
        var tools = new List<AITool>();
        
        // Add common tools
        tools.Add(AIFunctionFactory.Create(GetCurrentTime));
        tools.Add(AIFunctionFactory.Create(CalculateRiskScore));
        
        // Add agent-specific tools
        switch (agentType)
        {
            case "audit":
                tools.Add(AIFunctionFactory.Create(GetAuditFramework));
                tools.Add(AIFunctionFactory.Create(CalculateAuditRisk));
                break;
            case "compliance":
                tools.Add(AIFunctionFactory.Create(GetRegulatoryFramework));
                tools.Add(AIFunctionFactory.Create(CheckComplianceDatabase));
                break;
        }
        
        return tools;
    }
    
    [Description("Get the current date and time")]
    public static string GetCurrentTime()
    {
        return DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC");
    }
    
    [Description("Calculate risk score based on multiple factors")]
    public static decimal CalculateRiskScore(
        [Description("Financial impact (1-10)")] int financialImpact,
        [Description("Probability (1-10)")] int probability,
        [Description("Regulatory impact (1-10)")] int regulatoryImpact)
    {
        return (financialImpact * probability * regulatoryImpact) / 10m;
    }
}
```

## Benefits of Enhanced AI Service

### 1. **Multi-Model Flexibility**
- **GitHub Models**: Free tier for development, latest models (GPT-5, o3, etc.)
- **Hertze Server**: Your existing infrastructure, full control
- **Model Switching**: Easy configuration-based switching

### 2. **Advanced Capabilities**
- **Tool Calling**: AI can call your business functions
- **Multi-Turn Conversations**: Persistent conversation threads
- **Streaming Responses**: Real-time response streaming
- **Specialized Agents**: Different agents for different domains

### 3. **Production Ready**
- **Comprehensive Logging**: All AI requests logged to PostgreSQL
- **Caching**: Redis-backed response caching
- **Error Handling**: Robust error handling and retry logic
- **Performance Monitoring**: Request timing and success metrics

### 4. **Cost Optimization**
- **GitHub Models Free Tier**: Significant cost savings for development
- **Intelligent Routing**: Route simple queries to cheaper models
- **Caching**: Reduce duplicate AI calls

## Next Implementation Steps

1. **Install Packages** (5 minutes)
2. **Create Enhanced Interfaces** (30 minutes)
3. **Implement Agent Factory** (2 hours)
4. **Update AI Service Implementation** (3 hours)
5. **Add New API Endpoints** (2 hours)
6. **Update Blazor UI** (3 hours)
7. **Testing and Deployment** (2 hours)

**Total Estimated Time: 1-2 days**

## Testing Strategy

### 1. **Unit Tests**
- Test agent creation and configuration
- Test tool calling functionality
- Test error handling

### 2. **Integration Tests**
- Test GitHub Models integration
- Test Hertze server fallback
- Test conversation persistence

### 3. **Performance Tests**
- Response time benchmarks
- Concurrent user testing
- Memory usage profiling

## Security Considerations

1. **GitHub Token Security**: Store in Azure Key Vault or environment variables
2. **Request Sanitization**: Validate all user inputs
3. **Rate Limiting**: Implement per-user rate limits
4. **Audit Logging**: Log all AI interactions for security audits