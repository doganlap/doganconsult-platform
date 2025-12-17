using Microsoft.Extensions.Configuration;

namespace DoganConsult.AI.Infrastructure.Configuration;

public class AgentConfiguration
{
    public const string ConfigurationSection = "AIService";
    
    // Model providers
    public bool UseGitHubModels { get; set; } = true;
    public bool UseHertzeServer { get; set; } = true;
    public bool UseMicrosoftFoundry { get; set; } = false;
    
    // GitHub Models configuration
    public string GitHubToken { get; set; } = "";
    public string DefaultGitHubModel { get; set; } = "openai/gpt-4.1-mini";
    public string GitHubEndpoint { get; set; } = "https://models.github.ai/inference";
    
    // Hertze server configuration (existing)
    public string LlmServerEndpoint { get; set; } = "";
    public string ApiKey { get; set; } = "";
    public int TimeoutSeconds { get; set; } = 30;
    public int MaxRetries { get; set; } = 3;
    
    // Microsoft Foundry configuration
    public string AzureOpenAIEndpoint { get; set; } = "";
    public string AzureOpenAIKey { get; set; } = "";
    public string AzureOpenAIDeployment { get; set; } = "";
    
    // Agent-specific configurations
    public AgentConfig ConsultingAgent { get; set; } = new();
    public AgentConfig AuditAgent { get; set; } = new();
    public AgentConfig ComplianceAgent { get; set; } = new();
    
    // General AI settings
    public int MaxTokens { get; set; } = 4000;
    public decimal Temperature { get; set; } = 0.7m;
    public int MaxConcurrentRequests { get; set; } = 10;
    public bool EnableRequestLogging { get; set; } = true;
    public bool EnableResponseCaching { get; set; } = true;
    public int CacheDurationMinutes { get; set; } = 15;
    
    // Cost management
    public decimal DailySpendingLimit { get; set; } = 100.00m;
    public bool EnableCostTracking { get; set; } = true;
    
    // Model selection strategy
    public ModelSelectionStrategy SelectionStrategy { get; set; } = ModelSelectionStrategy.CostOptimized;
}

public class AgentConfig
{
    public string SystemPrompt { get; set; } = "";
    public string PreferredModel { get; set; } = "";
    public int MaxTokens { get; set; } = 4000;
    public decimal Temperature { get; set; } = 0.7m;
    public bool EnableTools { get; set; } = true;
    public List<string> AllowedTools { get; set; } = new();
}

public enum ModelSelectionStrategy
{
    CostOptimized,      // Choose cheapest model that meets requirements
    PerformanceFirst,   // Choose best performing model
    Balanced,          // Balance cost and performance
    UserSpecified      // User specifies model
}

public static class AgentDefaults
{
    public static AgentConfig ConsultingAgent => new()
    {
        SystemPrompt = """
            You are an expert management consultant with deep expertise in:
            - Strategic planning and organizational design
            - Process optimization and efficiency improvement
            - Change management and digital transformation
            - Performance measurement and KPI development
            - Industry best practices and benchmarking
            
            Provide actionable insights based on proven consulting frameworks.
            Always consider the business context and provide specific, measurable recommendations.
            Use structured analysis (SWOT, Porter's Five Forces, McKinsey 7S, etc.) when appropriate.
            """,
        PreferredModel = "openai/gpt-4.1",
        MaxTokens = 4000,
        Temperature = 0.7m,
        EnableTools = true,
        AllowedTools = new List<string>
        {
            "GetCurrentTime",
            "CalculateROI",
            "GetIndustryBenchmarks",
            "CreateActionPlan"
        }
    };
    
    public static AgentConfig AuditAgent => new()
    {
        SystemPrompt = """
            You are a senior audit professional with expertise in:
            - Internal and external audit procedures
            - Risk assessment and control evaluation
            - Regulatory compliance and standards (SOX, COSO, ISO)
            - Fraud detection and prevention
            - Audit report writing and findings documentation
            
            Follow professional auditing standards and provide evidence-based assessments.
            Always consider materiality, risk, and control effectiveness.
            Structure findings with clear recommendations for remediation.
            """,
        PreferredModel = "openai/gpt-4.1",
        MaxTokens = 4000,
        Temperature = 0.5m, // Lower temperature for more precise audit work
        EnableTools = true,
        AllowedTools = new List<string>
        {
            "GetCurrentTime",
            "CalculateRiskScore",
            "GetAuditFramework",
            "GenerateAuditProcedure",
            "CalculateMateriality"
        }
    };
    
    public static AgentConfig ComplianceAgent => new()
    {
        SystemPrompt = """
            You are a compliance expert specializing in:
            - Regulatory frameworks (GDPR, SOX, HIPAA, PCI-DSS, etc.)
            - Risk management and governance
            - Policy development and implementation
            - Compliance monitoring and reporting
            - Legal and regulatory requirement analysis
            
            Provide accurate, up-to-date regulatory guidance.
            Always reference specific regulations and requirements.
            Consider jurisdiction-specific requirements and cross-border implications.
            Focus on practical implementation and risk mitigation.
            """,
        PreferredModel = "openai/gpt-4.1",
        MaxTokens = 4000,
        Temperature = 0.3m, // Lower temperature for regulatory accuracy
        EnableTools = true,
        AllowedTools = new List<string>
        {
            "GetCurrentTime",
            "GetRegulatoryFramework",
            "CheckComplianceDatabase",
            "CalculateComplianceRisk",
            "GenerateComplianceChecklist"
        }
    };
}

// Extension methods for configuration
public static class AgentConfigurationExtensions
{
    public static AgentConfiguration ConfigureDefaults(this AgentConfiguration config)
    {
        config.ConsultingAgent = AgentDefaults.ConsultingAgent;
        config.AuditAgent = AgentDefaults.AuditAgent;
        config.ComplianceAgent = AgentDefaults.ComplianceAgent;
        
        return config;
    }
    
    public static bool IsGitHubModelsConfigured(this AgentConfiguration config)
    {
        return config.UseGitHubModels && !string.IsNullOrEmpty(config.GitHubToken);
    }
    
    public static bool IsHertzeServerConfigured(this AgentConfiguration config)
    {
        return config.UseHertzeServer && !string.IsNullOrEmpty(config.LlmServerEndpoint);
    }
    
    public static bool IsMicrosoftFoundryConfigured(this AgentConfiguration config)
    {
        return config.UseMicrosoftFoundry && 
               !string.IsNullOrEmpty(config.AzureOpenAIEndpoint) && 
               !string.IsNullOrEmpty(config.AzureOpenAIKey);
    }
}