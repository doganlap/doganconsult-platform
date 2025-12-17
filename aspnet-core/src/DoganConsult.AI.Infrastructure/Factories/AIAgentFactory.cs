using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using OpenAI;
using OpenAI.Chat;
using DoganConsult.AI.Infrastructure.Configuration;

namespace DoganConsult.AI.Infrastructure.Factories;

public class AIAgentFactory
{
    private readonly AgentConfiguration _config;
    private readonly ILogger<AIAgentFactory> _logger;
    private readonly OpenAIClient? _githubClient;
    private readonly OpenAIClient? _hertzeClient;

    public AIAgentFactory(
        IOptions<AgentConfiguration> config,
        ILogger<AIAgentFactory> logger)
    {
        _config = config.Value;
        _logger = logger;
        
        // Initialize GitHub Models client
        if (!string.IsNullOrEmpty(_config.GitHubToken))
        {
            _githubClient = new OpenAIClient(new System.ClientModel.ApiKeyCredential(_config.GitHubToken), new OpenAIClientOptions
            {
                Endpoint = new Uri(_config.GitHubEndpoint)
            });
        }
        
        // Initialize Hertze server client
        if (!string.IsNullOrEmpty(_config.LlmServerEndpoint))
        {
            _hertzeClient = new OpenAIClient(new System.ClientModel.ApiKeyCredential("dummy-key"), new OpenAIClientOptions
            {
                Endpoint = new Uri(_config.LlmServerEndpoint)
            });
        }
    }

    public SimpleAIAgent CreateConsultingAgent()
    {
        return CreateAgent("consulting", _config.DefaultGitHubModel);
    }

    public SimpleAIAgent CreateAuditAgent()
    {
        return CreateAgent("audit", _config.DefaultGitHubModel);
    }

    public SimpleAIAgent CreateComplianceAgent()
    {
        return CreateAgent("compliance", "hertze-model");
    }

    private SimpleAIAgent CreateAgent(string agentType, string modelName)
    {
        try
        {
            var systemPrompt = GetSystemPromptForAgent(agentType);
            
            var client = modelName.Contains("hertze") ? _hertzeClient : _githubClient;
            
            var agent = new SimpleAIAgent(client, modelName, systemPrompt, _logger);

            _logger.LogInformation("Created {AgentType} agent with model {ModelName}", agentType, modelName);
            
            return agent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating {AgentType} agent", agentType);
            // Return mock agent as fallback
            return new SimpleAIAgent(null, "mock", GetSystemPromptForAgent(agentType), _logger);
        }
    }

    private string GetSystemPromptForAgent(string agentType)
    {
        return agentType.ToLower() switch
        {
            "consulting" => @"
                You are a professional business consulting agent specialized in strategic planning, 
                process optimization, and organizational development. Provide expert advice based on 
                industry best practices and proven methodologies. Always consider the business context, 
                stakeholder interests, and measurable outcomes in your recommendations.
                
                Focus on:
                - Strategic analysis and planning
                - Process improvement recommendations
                - Risk assessment and mitigation
                - Change management guidance
                - Performance metrics and KPIs
                
                Maintain a professional, analytical tone and provide actionable insights.",
                
            "audit" => @"
                You are a certified audit professional specializing in financial, operational, and 
                compliance auditing. Your role is to assess risks, evaluate controls, and provide 
                objective analysis of business processes and financial statements.
                
                Focus on:
                - Risk assessment and control evaluation
                - Compliance verification
                - Financial statement analysis
                - Internal control testing
                - Audit finding documentation
                - Remediation recommendations
                
                Maintain objectivity, follow audit standards, and provide clear, evidence-based conclusions.",
                
            "compliance" => @"
                You are a compliance specialist expert in regulatory requirements, industry standards, 
                and legal frameworks across multiple jurisdictions. Help organizations navigate complex 
                compliance landscapes and maintain regulatory adherence.
                
                Focus on:
                - Regulatory requirement analysis
                - Compliance gap identification
                - Policy and procedure development
                - Training and awareness programs
                - Monitoring and reporting frameworks
                - Incident response and remediation
                
                Stay current with regulatory changes and provide practical compliance solutions.",
                
            _ => "You are a helpful business assistant. Provide professional and accurate information based on the context provided."
        };
    }
}

// Simple AI Agent implementation using OpenAI directly
public class SimpleAIAgent
{
    private readonly OpenAIClient? _client;
    private readonly string _modelName;
    private readonly string _systemPrompt;
    private readonly ILogger _logger;
    private readonly Dictionary<string, List<ChatMessage>> _conversations;

    public SimpleAIAgent(OpenAIClient? client, string modelName, string systemPrompt, ILogger logger)
    {
        _client = client;
        _modelName = modelName;
        _systemPrompt = systemPrompt;
        _logger = logger;
        _conversations = new Dictionary<string, List<ChatMessage>>();
    }

    public async Task<string> RunAsync(string message, string? threadId = null)
    {
        if (_client == null || _modelName == "mock")
        {
            return $"Mock response for: {message}";
        }

        try
        {
            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(_systemPrompt)
            };

            // Add conversation history if threadId provided
            if (!string.IsNullOrEmpty(threadId) && _conversations.ContainsKey(threadId))
            {
                messages.AddRange(_conversations[threadId]);
            }

            messages.Add(new UserChatMessage(message));

            var response = await _client.GetChatClient(_modelName).CompleteChatAsync(messages);
            var responseText = response.Value.Content[0].Text;

            // Store conversation history
            if (!string.IsNullOrEmpty(threadId))
            {
                if (!_conversations.ContainsKey(threadId))
                    _conversations[threadId] = new List<ChatMessage>();
                
                _conversations[threadId].Add(new UserChatMessage(message));
                _conversations[threadId].Add(new AssistantChatMessage(responseText));
            }

            return responseText;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling AI model {ModelName}", _modelName);
            return $"Error processing request: {ex.Message}";
        }
    }

    public string GetNewThreadId()
    {
        return Guid.NewGuid().ToString();
    }
}