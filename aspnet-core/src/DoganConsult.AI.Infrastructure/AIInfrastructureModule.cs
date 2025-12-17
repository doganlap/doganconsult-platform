using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Distributed;
using Volo.Abp.Modularity;
using DoganConsult.AI.AIRequests;
using DoganConsult.AI.Application.Contracts.Services;
using DoganConsult.AI.Infrastructure.Services;
using DoganConsult.AI.Infrastructure.Factories;
using DoganConsult.AI.Infrastructure.Configuration;

namespace DoganConsult.AI.Infrastructure;

[DependsOn(
    typeof(AIDomainModule)
)]
public class AIInfrastructureModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        
        // Configure AI Agent settings
        context.Services.Configure<AgentConfiguration>(configuration.GetSection(AgentConfiguration.ConfigurationSection));
        
        // Register HTTP client for external API calls
        context.Services.AddHttpClient();
        
        // Register existing LLM service (for backward compatibility)
        context.Services.AddTransient<DoganConsult.AI.AIRequests.ILlmService, LlmService>();
        
        // Register enhanced AI services
        context.Services.AddTransient<AIAgentFactory>();
        context.Services.AddTransient<IEnhancedAIService, EnhancedAIService>();
        
        // Register distributed caching for response caching - using Redis
        context.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis") ?? 
                                  configuration.GetValue<string>("Redis:Configuration");
            options.InstanceName = configuration.GetValue<string>("Redis:InstanceName");
        });
    }
}
