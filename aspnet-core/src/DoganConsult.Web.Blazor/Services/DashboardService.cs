using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;

namespace DoganConsult.Web.Blazor.Services;

public class DashboardService : ITransientDependency
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<DashboardService> _logger;
    private readonly IConfiguration _configuration;
    
    private readonly string _gatewayUrl;
    private readonly string _organizationUrl;
    private readonly string _workspaceUrl;
    private readonly string _documentUrl;
    private readonly string _auditUrl;
    private readonly string _aiUrl;

    public DashboardService(HttpClient httpClient, ILogger<DashboardService> logger, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        _configuration = configuration;
        
        // Use Gateway URL from configuration (defaults to RemoteServices:Default:BaseUrl)
        _gatewayUrl = configuration["RemoteServices:Default:BaseUrl"] ?? "http://localhost:5000";
        
        // Build service URLs from gateway base URL
        _organizationUrl = $"{_gatewayUrl}/api/organization";
        _workspaceUrl = $"{_gatewayUrl}/api/workspace";
        _documentUrl = $"{_gatewayUrl}/api/document";
        _auditUrl = $"{_gatewayUrl}/api/audit";
        _aiUrl = $"{_gatewayUrl}/api/ai";
        
        _logger.LogInformation("Dashboard Service initialized with Gateway URL: {GatewayUrl}", _gatewayUrl);
    }

    public async Task<DashboardSummaryDto> GetDashboardSummaryAsync()
    {
        var summary = new DashboardSummaryDto();
        
        try
        {
            // Fetch counts from various services in parallel
            var tasks = new List<Task>
            {
                Task.Run(async () => summary.TotalOrganizations = await GetCountAsync($"{_organizationUrl}/organizations/count")),
                Task.Run(async () => summary.TotalWorkspaces = await GetCountAsync($"{_workspaceUrl}/workspaces/count")),
                Task.Run(async () => summary.TotalDocuments = await GetCountAsync($"{_documentUrl}/documents/count")),
                Task.Run(async () => summary.PendingApprovals = await GetCountAsync($"{_auditUrl}/approvals/pending-count")),
            };
            
            await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error fetching dashboard summary, using defaults");
        }
        
        return summary;
    }

    private async Task<int> GetCountAsync(string url)
    {
        try
        {
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                if (int.TryParse(content, out var count))
                    return count;
            }
        }
        catch
        {
            // Return 0 on error
        }
        return 0;
    }

    public async Task<List<ActivityItemDto>> GetRecentActivitiesAsync(int count = 10)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<ActivityItemDto>>($"{_auditUrl}/activities/recent?count={count}");
            return response ?? new List<ActivityItemDto>();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error fetching recent activities");
            return GetMockActivities();
        }
    }

    public async Task<List<ChartDataPointDto>> GetOrganizationTrendsAsync(int days = 30)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<ChartDataPointDto>>($"{_organizationUrl}/statistics/trends?days={days}");
            return response ?? GetMockTrendData();
        }
        catch
        {
            return GetMockTrendData();
        }
    }

    public async Task<List<ChartDataPointDto>> GetDocumentTrendsAsync(int days = 30)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<ChartDataPointDto>>($"{_documentUrl}/statistics/trends?days={days}");
            return response ?? GetMockTrendData();
        }
        catch
        {
            return GetMockTrendData();
        }
    }

    public async Task<List<PivotDataDto>> GetApprovalPivotDataAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<PivotDataDto>>($"{_auditUrl}/approvals/pivot");
            return response ?? GetMockPivotData();
        }
        catch
        {
            return GetMockPivotData();
        }
    }

    public async Task<UserRoleGuidanceDto> GetUserRoleGuidanceAsync(string role)
    {
        // Return role-specific guidance
        return role?.ToLower() switch
        {
            "admin" => new UserRoleGuidanceDto
            {
                Role = "Administrator",
                Description = "Full system access with administrative privileges",
                Responsibilities = new List<string>
                {
                    "Manage users and roles across all organizations",
                    "Configure system settings and permissions",
                    "Monitor system health and performance",
                    "Review and approve critical changes",
                    "Access all audit logs and reports"
                },
                QuickActions = new List<QuickActionDto>
                {
                    new() { Title = "User Management", Icon = "fas fa-users-cog", Route = "/identity/users", Description = "Manage users and permissions" },
                    new() { Title = "System Settings", Icon = "fas fa-cogs", Route = "/settings", Description = "Configure system parameters" },
                    new() { Title = "Audit Logs", Icon = "fas fa-history", Route = "/audit-logs", Description = "Review system activities" },
                    new() { Title = "Approvals", Icon = "fas fa-check-double", Route = "/approvals", Description = "Process pending approvals" }
                },
                KnowledgeBase = new List<KnowledgeItemDto>
                {
                    new() { Title = "Admin Guide", Category = "Documentation", Url = "/docs/admin-guide", Icon = "fas fa-book" },
                    new() { Title = "Security Best Practices", Category = "Security", Url = "/docs/security", Icon = "fas fa-shield-alt" },
                    new() { Title = "User Management Tutorial", Category = "Tutorial", Url = "/docs/user-management", Icon = "fas fa-graduation-cap" }
                }
            },
            "manager" => new UserRoleGuidanceDto
            {
                Role = "Manager",
                Description = "Organization and team management capabilities",
                Responsibilities = new List<string>
                {
                    "Oversee organization activities and performance",
                    "Manage team members and workspaces",
                    "Approve documents and workflow requests",
                    "Generate reports and analytics",
                    "Coordinate with other departments"
                },
                QuickActions = new List<QuickActionDto>
                {
                    new() { Title = "My Organization", Icon = "fas fa-building", Route = "/organizations", Description = "Manage your organization" },
                    new() { Title = "Team Workspaces", Icon = "fas fa-th-large", Route = "/workspaces", Description = "View team workspaces" },
                    new() { Title = "Pending Approvals", Icon = "fas fa-tasks", Route = "/approvals", Description = "Review pending items" },
                    new() { Title = "Reports", Icon = "fas fa-chart-bar", Route = "/reports", Description = "View analytics and reports" }
                },
                KnowledgeBase = new List<KnowledgeItemDto>
                {
                    new() { Title = "Manager Handbook", Category = "Documentation", Url = "/docs/manager-handbook", Icon = "fas fa-book" },
                    new() { Title = "Approval Workflow Guide", Category = "Process", Url = "/docs/approval-workflow", Icon = "fas fa-sitemap" },
                    new() { Title = "Team Collaboration Tips", Category = "Best Practices", Url = "/docs/collaboration", Icon = "fas fa-users" }
                }
            },
            "auditor" => new UserRoleGuidanceDto
            {
                Role = "Auditor",
                Description = "Compliance and audit oversight responsibilities",
                Responsibilities = new List<string>
                {
                    "Review and verify compliance requirements",
                    "Conduct internal audits and assessments",
                    "Document findings and recommendations",
                    "Track remediation progress",
                    "Generate compliance reports"
                },
                QuickActions = new List<QuickActionDto>
                {
                    new() { Title = "Audit Dashboard", Icon = "fas fa-clipboard-check", Route = "/audit", Description = "View audit overview" },
                    new() { Title = "Compliance Checks", Icon = "fas fa-balance-scale", Route = "/compliance", Description = "Run compliance checks" },
                    new() { Title = "Findings", Icon = "fas fa-search", Route = "/findings", Description = "Review audit findings" },
                    new() { Title = "Reports", Icon = "fas fa-file-alt", Route = "/audit-reports", Description = "Generate audit reports" }
                },
                KnowledgeBase = new List<KnowledgeItemDto>
                {
                    new() { Title = "Audit Standards", Category = "Standards", Url = "/docs/audit-standards", Icon = "fas fa-gavel" },
                    new() { Title = "Compliance Framework", Category = "Framework", Url = "/docs/compliance-framework", Icon = "fas fa-project-diagram" },
                    new() { Title = "Risk Assessment Guide", Category = "Methodology", Url = "/docs/risk-assessment", Icon = "fas fa-exclamation-triangle" }
                }
            },
            _ => new UserRoleGuidanceDto
            {
                Role = "User",
                Description = "Standard user with basic access",
                Responsibilities = new List<string>
                {
                    "Access assigned workspaces and documents",
                    "Submit requests and approvals",
                    "Collaborate with team members",
                    "Track personal tasks and activities"
                },
                QuickActions = new List<QuickActionDto>
                {
                    new() { Title = "My Workspace", Icon = "fas fa-desktop", Route = "/workspaces", Description = "Access your workspace" },
                    new() { Title = "Documents", Icon = "fas fa-folder-open", Route = "/documents", Description = "View your documents" },
                    new() { Title = "My Requests", Icon = "fas fa-paper-plane", Route = "/my-requests", Description = "Track your requests" },
                    new() { Title = "Help Center", Icon = "fas fa-question-circle", Route = "/help", Description = "Get assistance" }
                },
                KnowledgeBase = new List<KnowledgeItemDto>
                {
                    new() { Title = "Getting Started", Category = "Guide", Url = "/docs/getting-started", Icon = "fas fa-rocket" },
                    new() { Title = "User Manual", Category = "Documentation", Url = "/docs/user-manual", Icon = "fas fa-book-open" },
                    new() { Title = "FAQ", Category = "Support", Url = "/docs/faq", Icon = "fas fa-question" }
                }
            }
        };
    }

    public async Task<AIAgentResponseDto> GetPersonalizedAIGuidanceAsync(PersonalizedAgentRequestDto request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{_aiUrl}/personalized-guidance", request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<AIAgentResponseDto>() ?? GetMockAIGuidance(request);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error getting AI guidance, using mock data");
        }
        
        return GetMockAIGuidance(request);
    }

    private AIAgentResponseDto GetMockAIGuidance(PersonalizedAgentRequestDto request)
    {
        return new AIAgentResponseDto
        {
            Greeting = $"Hello! Based on your {request.Role} role, here's what I recommend for today:",
            Recommendations = new List<RecommendationDto>
            {
                new() { Priority = "High", Title = "Review Pending Approvals", Description = "You have items waiting for your review", Action = "Go to Approvals", Route = "/approvals" },
                new() { Priority = "Medium", Title = "Check Recent Documents", Description = "New documents have been shared with you", Action = "View Documents", Route = "/documents" },
                new() { Priority = "Low", Title = "Update Your Profile", Description = "Keep your profile information current", Action = "Edit Profile", Route = "/profile" }
            },
            Tips = new List<string>
            {
                "Use keyboard shortcuts to navigate faster (press ? for help)",
                "Set up notifications to stay informed about important updates",
                "Explore the knowledge base for best practices"
            },
            DailyFocus = "Focus on completing high-priority approvals and reviewing team documents."
        };
    }

    private List<ActivityItemDto> GetMockActivities()
    {
        return new List<ActivityItemDto>
        {
            new() { Id = Guid.NewGuid(), Type = "Document", Action = "Created", Description = "New document uploaded", Timestamp = DateTime.UtcNow.AddMinutes(-15), User = "John Doe" },
            new() { Id = Guid.NewGuid(), Type = "Approval", Action = "Approved", Description = "Request #1234 approved", Timestamp = DateTime.UtcNow.AddMinutes(-45), User = "Jane Smith" },
            new() { Id = Guid.NewGuid(), Type = "Workspace", Action = "Updated", Description = "Workspace settings modified", Timestamp = DateTime.UtcNow.AddHours(-2), User = "Mike Johnson" },
            new() { Id = Guid.NewGuid(), Type = "Organization", Action = "Created", Description = "New organization registered", Timestamp = DateTime.UtcNow.AddHours(-5), User = "Admin" },
            new() { Id = Guid.NewGuid(), Type = "User", Action = "Login", Description = "User logged in", Timestamp = DateTime.UtcNow.AddHours(-8), User = "Sarah Wilson" }
        };
    }

    private List<ChartDataPointDto> GetMockTrendData()
    {
        var data = new List<ChartDataPointDto>();
        var random = new Random();
        for (int i = 29; i >= 0; i--)
        {
            data.Add(new ChartDataPointDto
            {
                Label = DateTime.UtcNow.AddDays(-i).ToString("MMM dd"),
                Value = random.Next(5, 50)
            });
        }
        return data;
    }

    private List<PivotDataDto> GetMockPivotData()
    {
        return new List<PivotDataDto>
        {
            new() { Row = "Organization A", Column = "Pending", Value = 5 },
            new() { Row = "Organization A", Column = "Approved", Value = 25 },
            new() { Row = "Organization A", Column = "Rejected", Value = 3 },
            new() { Row = "Organization B", Column = "Pending", Value = 8 },
            new() { Row = "Organization B", Column = "Approved", Value = 42 },
            new() { Row = "Organization B", Column = "Rejected", Value = 7 },
            new() { Row = "Organization C", Column = "Pending", Value = 2 },
            new() { Row = "Organization C", Column = "Approved", Value = 18 },
            new() { Row = "Organization C", Column = "Rejected", Value = 1 }
        };
    }
}

// DTOs for Dashboard
public class DashboardSummaryDto
{
    public int TotalOrganizations { get; set; }
    public int TotalWorkspaces { get; set; }
    public int TotalDocuments { get; set; }
    public int TotalUsers { get; set; }
    public int PendingApprovals { get; set; }
    public int ActiveTasks { get; set; }
    public double SystemHealthScore { get; set; } = 98.5;
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}

public class ActivityItemDto
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string User { get; set; } = string.Empty;
}

public class ChartDataPointDto
{
    public string Label { get; set; } = string.Empty;
    public double Value { get; set; }
    public string? Color { get; set; }
}

public class PivotDataDto
{
    public string Row { get; set; } = string.Empty;
    public string Column { get; set; } = string.Empty;
    public double Value { get; set; }
}

public class UserRoleGuidanceDto
{
    public string Role { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Responsibilities { get; set; } = new();
    public List<QuickActionDto> QuickActions { get; set; } = new();
    public List<KnowledgeItemDto> KnowledgeBase { get; set; } = new();
}

public class QuickActionDto
{
    public string Title { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class KnowledgeItemDto
{
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
}

public class PersonalizedAgentRequestDto
{
    public Guid UserId { get; set; }
    public string Role { get; set; } = string.Empty;
    public string? Department { get; set; }
    public List<string>? Permissions { get; set; }
    public string? Context { get; set; }
}

public class AIAgentResponseDto
{
    public string Greeting { get; set; } = string.Empty;
    public List<RecommendationDto> Recommendations { get; set; } = new();
    public List<string> Tips { get; set; } = new();
    public string DailyFocus { get; set; } = string.Empty;
}

public class RecommendationDto
{
    public string Priority { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
}
