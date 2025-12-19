using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DoganConsult.Web.Blazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Configuration;

namespace DoganConsult.Web.Blazor.Components.Pages;

public partial class Index : WebComponentBase
{
    [Inject] private DashboardService DashboardService { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;
    [Inject] private IConfiguration Configuration { get; set; } = default!;

    // State
    private string ActiveTab { get; set; } = "overview";
    private bool IsLoading { get; set; } = true;
    private string CurrentUserRole { get; set; } = "User";
    private Guid CurrentUserId { get; set; } = Guid.Empty;
    private string CurrentUserName { get; set; } = "User";
    private string CurrentUserTitle { get; set; } = "Team Member";
    private string CurrentUserEmail { get; set; } = "";

    // Dashboard Data
    private DashboardSummaryDto Summary { get; set; } = new();
    private UserRoleGuidanceDto RoleGuidance { get; set; } = new();
    private AIAgentResponseDto AIResponse { get; set; } = new();
    private List<ActivityItemDto> Activities { get; set; } = new();
    private List<ChartDataPointDto> OrganizationTrends { get; set; } = new();
    private List<ChartDataPointDto> DocumentTrends { get; set; } = new();
    private List<PivotDataDto> PivotData { get; set; } = new();

    // Filter States
    private int SelectedTimeRange { get; set; } = 30;
    private string SelectedChartType { get; set; } = "line";
    private string SelectedMetric { get; set; } = "all";
    private string PivotRowField { get; set; } = "organization";
    private string PivotColumnField { get; set; } = "status";
    private string PivotAggregation { get; set; } = "count";
    private string PivotSortField { get; set; } = "";
    private bool PivotSortAsc { get; set; } = true;
    private string KnowledgeSearchQuery { get; set; } = "";
    private string KnowledgeCategory { get; set; } = "";
    private string ActivitySearchQuery { get; set; } = "";
    private string ActivityTypeFilter { get; set; } = "";

    // AI Assistant
    private bool ShowAIAssistant { get; set; } = false;
    private bool IsAIChatMinimized { get; set; } = false;
    private string UserMessage { get; set; } = "";
    private bool IsSendingMessage { get; set; } = false;
    private List<ChatMessage> ChatMessages { get; set; } = new();

    [Inject] private DemoService DemoService { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        await LoadUserContext();
        await LoadDashboardData();
        await LoadDemoStatisticsAsync();
        await InitializeSignalRAsync();
    }

    private async Task InitializeSignalRAsync()
    {
        try
        {
            await DemoService.InitializeSignalRAsync();
            
            DemoService.OnDemoCreated += (demo) =>
            {
                InvokeAsync(async () =>
                {
                    await LoadDemoStatisticsAsync();
                    StateHasChanged();
                });
            };

            DemoService.OnDemoApproved += (demo) =>
            {
                InvokeAsync(async () =>
                {
                    await LoadDemoStatisticsAsync();
                    StateHasChanged();
                });
            };

            DemoService.OnDemoStatusChanged += (demo) =>
            {
                InvokeAsync(async () =>
                {
                    await LoadDemoStatisticsAsync();
                    StateHasChanged();
                });
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"SignalR connection failed: {ex.Message}");
        }
    }

    private async Task LoadUserContext()
    {
        try
        {
            var authState = await AuthStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (Guid.TryParse(userIdClaim, out var userId))
                {
                    CurrentUserId = userId;
                }

                // Get user name from claims
                CurrentUserName = user.FindFirst(ClaimTypes.Name)?.Value 
                    ?? user.FindFirst("preferred_username")?.Value 
                    ?? user.FindFirst("name")?.Value 
                    ?? "User";
                
                // Get email
                CurrentUserEmail = user.FindFirst(ClaimTypes.Email)?.Value 
                    ?? user.FindFirst("email")?.Value 
                    ?? "";

                // Get role from claims
                var roleClaim = user.FindFirst(ClaimTypes.Role)?.Value;
                CurrentUserRole = roleClaim ?? "User";

                // Set title based on role
                CurrentUserTitle = CurrentUserRole switch
                {
                    "admin" or "Admin" or "Administrator" => "System Administrator",
                    "manager" or "Manager" => "Team Manager",
                    "auditor" or "Auditor" => "Compliance Auditor",
                    "developer" or "Developer" => "Software Developer",
                    "analyst" or "Analyst" => "Business Analyst",
                    _ => "Team Member"
                };
            }
        }
        catch
        {
            CurrentUserRole = "User";
            CurrentUserName = "User";
            CurrentUserTitle = "Team Member";
        }
    }

    private async Task LoadDashboardData()
    {
        IsLoading = true;

        try
        {
            // Load all data in parallel
            var summaryTask = DashboardService.GetDashboardSummaryAsync();
            var guidanceTask = DashboardService.GetUserRoleGuidanceAsync(CurrentUserRole);
            var activitiesTask = DashboardService.GetRecentActivitiesAsync();
            var trendsTask = DashboardService.GetOrganizationTrendsAsync(SelectedTimeRange);
            var pivotTask = DashboardService.GetApprovalPivotDataAsync();
            var aiTask = DashboardService.GetPersonalizedAIGuidanceAsync(new PersonalizedAgentRequestDto
            {
                UserId = CurrentUserId,
                Role = CurrentUserRole
            });

            await Task.WhenAll(summaryTask, guidanceTask, activitiesTask, trendsTask, pivotTask, aiTask);

            Summary = await summaryTask;
            RoleGuidance = await guidanceTask;
            Activities = await activitiesTask;
            OrganizationTrends = await trendsTask;
            PivotData = await pivotTask;
            AIResponse = await aiTask;

            // Initialize chat with personalized AI greeting
            ChatMessages.Add(new ChatMessage
            {
                IsUser = false,
                Content = $"Hello {CurrentUserName}! \ud83d\udc4b I'm your DC OS AI Agent, personalized for your {CurrentUserTitle} role. As a {CurrentUserRole}, I can help you with:\n\n\u2022 Managing your pending tasks and approvals\n\u2022 Navigating the platform efficiently\n\u2022 Answering questions about your responsibilities\n\u2022 Providing role-specific guidance\n\nHow can I assist you today?",
                Timestamp = DateTime.Now
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading dashboard: {ex.Message}");
            // Set defaults
            Summary = new DashboardSummaryDto();
            RoleGuidance = await DashboardService.GetUserRoleGuidanceAsync("user");
            AIResponse = new AIAgentResponseDto
            {
                Greeting = $"Welcome to DC OS, {CurrentUserName}!",
                DailyFocus = "Explore your personalized dashboard to get started with your tasks.",
                Recommendations = new List<RecommendationDto>(),
                Tips = new List<string> { "Use the tabs below to navigate different features." }
            };
        }

        IsLoading = false;
        StateHasChanged();
    }

    private void SwitchTab(string tab)
    {
        ActiveTab = tab;
        StateHasChanged();
    }

    private void NavigateTo(string route)
    {
        NavigationManager.NavigateTo(route);
    }

    private string GetPriorityClass(string priority)
    {
        return priority?.ToLower() switch
        {
            "high" => "border-danger",
            "medium" => "border-warning",
            "low" => "border-info",
            _ => ""
        };
    }

    private string GetPriorityBadgeClass(string priority)
    {
        return priority?.ToLower() switch
        {
            "high" => "bg-danger",
            "medium" => "bg-warning text-dark",
            "low" => "bg-info",
            _ => "bg-secondary"
        };
    }

    private string GetChartPoints(List<ChartDataPointDto> data)
    {
        if (!data.Any()) return "";

        var points = new List<string>();
        var lastTen = data.TakeLast(10).ToList();
        for (int i = 0; i < lastTen.Count; i++)
        {
            var x = 80 + i * 50;
            var y = 250 - (lastTen[i].Value / 50.0 * 200);
            points.Add($"{x},{y}");
        }
        return string.Join(" ", points);
    }

    // Pivot Table Methods
    private List<string> GetPivotRows()
    {
        var rows = PivotData.Select(x => x.Row).Distinct().ToList();

        if (!string.IsNullOrEmpty(PivotSortField))
        {
            rows = PivotSortField switch
            {
                "row" => PivotSortAsc ? rows.OrderBy(x => x).ToList() : rows.OrderByDescending(x => x).ToList(),
                "pending" => PivotSortAsc
                    ? rows.OrderBy(x => GetPivotValue(x, "Pending")).ToList()
                    : rows.OrderByDescending(x => GetPivotValue(x, "Pending")).ToList(),
                "approved" => PivotSortAsc
                    ? rows.OrderBy(x => GetPivotValue(x, "Approved")).ToList()
                    : rows.OrderByDescending(x => GetPivotValue(x, "Approved")).ToList(),
                "rejected" => PivotSortAsc
                    ? rows.OrderBy(x => GetPivotValue(x, "Rejected")).ToList()
                    : rows.OrderByDescending(x => GetPivotValue(x, "Rejected")).ToList(),
                _ => rows
            };
        }

        return rows;
    }

    private double GetPivotValue(string row, string column)
    {
        return PivotData.FirstOrDefault(x => x.Row == row && x.Column == column)?.Value ?? 0;
    }

    private void SortPivot(string field)
    {
        if (PivotSortField == field)
        {
            PivotSortAsc = !PivotSortAsc;
        }
        else
        {
            PivotSortField = field;
            PivotSortAsc = true;
        }
        StateHasChanged();
    }

    private async Task RefreshPivotData()
    {
        PivotData = await DashboardService.GetApprovalPivotDataAsync();
        StateHasChanged();
    }

    private void ExportPivotToCsv()
    {
        // Implementation for CSV export
        Console.WriteLine("Exporting to CSV...");
    }

    private void ExportPivotToExcel()
    {
        // Implementation for Excel export
        Console.WriteLine("Exporting to Excel...");
    }

    // Knowledge Base Methods
    private List<KnowledgeItemDto> GetFilteredKnowledge()
    {
        var items = RoleGuidance.KnowledgeBase;

        if (!string.IsNullOrEmpty(KnowledgeSearchQuery))
        {
            items = items.Where(x =>
                x.Title.Contains(KnowledgeSearchQuery, StringComparison.OrdinalIgnoreCase) ||
                x.Category.Contains(KnowledgeSearchQuery, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        if (!string.IsNullOrEmpty(KnowledgeCategory))
        {
            items = items.Where(x => x.Category == KnowledgeCategory).ToList();
        }

        return items;
    }

    // Activity Methods
    private List<ActivityItemDto> GetFilteredActivities()
    {
        var items = Activities;

        if (!string.IsNullOrEmpty(ActivitySearchQuery))
        {
            items = items.Where(x =>
                x.Description.Contains(ActivitySearchQuery, StringComparison.OrdinalIgnoreCase) ||
                x.User.Contains(ActivitySearchQuery, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        if (!string.IsNullOrEmpty(ActivityTypeFilter))
        {
            items = items.Where(x => x.Type == ActivityTypeFilter).ToList();
        }

        return items;
    }

    private async Task RefreshActivities()
    {
        Activities = await DashboardService.GetRecentActivitiesAsync();
        StateHasChanged();
    }

    private string GetActivityIcon(string type)
    {
        return type?.ToLower() switch
        {
            "document" => "fas fa-file-alt",
            "approval" => "fas fa-check-circle",
            "workspace" => "fas fa-th-large",
            "organization" => "fas fa-building",
            "user" => "fas fa-user",
            _ => "fas fa-circle"
        };
    }

    private string GetActivityIconClass(string type)
    {
        return type?.ToLower() switch
        {
            "document" => "bg-success",
            "approval" => "bg-warning",
            "workspace" => "bg-info",
            "organization" => "bg-primary",
            "user" => "bg-secondary",
            _ => "bg-dark"
        };
    }

    private string GetTimeAgo(DateTime timestamp)
    {
        var diff = DateTime.UtcNow - timestamp;

        if (diff.TotalMinutes < 1) return "just now";
        if (diff.TotalMinutes < 60) return $"{(int)diff.TotalMinutes}m ago";
        if (diff.TotalHours < 24) return $"{(int)diff.TotalHours}h ago";
        if (diff.TotalDays < 7) return $"{(int)diff.TotalDays}d ago";
        return timestamp.ToString("MMM dd");
    }

    // Chart Methods
    private async Task RefreshCharts()
    {
        OrganizationTrends = await DashboardService.GetOrganizationTrendsAsync(SelectedTimeRange);
        DocumentTrends = await DashboardService.GetDocumentTrendsAsync(SelectedTimeRange);
        StateHasChanged();
    }

    private void ExportChartData()
    {
        Console.WriteLine("Exporting chart data...");
    }

    // AI Assistant Methods
    private void OpenAIAssistant()
    {
        ShowAIAssistant = true;
        StateHasChanged();
    }

    private void CloseAIAssistant()
    {
        ShowAIAssistant = false;
        StateHasChanged();
    }

    private async Task HandleChatKeyPress(KeyboardEventArgs e)
    {
        if (e.Key == "Enter" && !string.IsNullOrWhiteSpace(UserMessage))
        {
            await SendMessage();
        }
    }

    private async Task SendMessage()
    {
        if (string.IsNullOrWhiteSpace(UserMessage) || IsSendingMessage) return;

        var message = UserMessage;
        UserMessage = "";
        IsSendingMessage = true;
        StateHasChanged();

        // Add user message with timestamp
        ChatMessages.Add(new ChatMessage { IsUser = true, Content = message, Timestamp = DateTime.Now });

        try
        {
            // Get AI response
            var response = await DashboardService.GetPersonalizedAIGuidanceAsync(new PersonalizedAgentRequestDto
            {
                UserId = CurrentUserId,
                Role = CurrentUserRole,
                Context = message
            });

            // Add AI response with timestamp
            var aiMessage = GenerateAIResponse(message, response);
            ChatMessages.Add(new ChatMessage { IsUser = false, Content = aiMessage, Timestamp = DateTime.Now });
        }
        catch
        {
            ChatMessages.Add(new ChatMessage
            {
                IsUser = false,
                Content = $"I apologize {CurrentUserName}, but I'm having trouble processing your request right now. Please try again later.",
                Timestamp = DateTime.Now
            });
        }

        IsSendingMessage = false;
        StateHasChanged();
    }

    private string GenerateAIResponse(string userMessage, AIAgentResponseDto guidance)
    {
        var lowerMessage = userMessage.ToLower();

        if (lowerMessage.Contains("approval") || lowerMessage.Contains("approve"))
        {
            return "To manage approvals, go to the Approvals page from the menu. You can view pending requests, approve or reject them, and track the approval history. Would you like me to show you how to create a new approval request?";
        }

        if (lowerMessage.Contains("document") || lowerMessage.Contains("file"))
        {
            return "You can manage documents in the Documents section. You can upload new files, organize them in folders, and share them with team members. The AI can also analyze documents for you if needed.";
        }

        if (lowerMessage.Contains("organization") || lowerMessage.Contains("company"))
        {
            return "Organizations are the top-level entities in the platform. Each organization can have multiple workspaces, users, and documents. Go to Organizations from the menu to view or manage them.";
        }

        if (lowerMessage.Contains("workspace"))
        {
            return "Workspaces help you organize your work within an organization. Each workspace can have its own set of documents, tasks, and team members. Navigate to Workspaces to create or manage them.";
        }

        if (lowerMessage.Contains("help") || lowerMessage.Contains("how"))
        {
            return "I'm here to help! You can ask me about:\n• Managing approvals and workflows\n• Working with documents\n• Organization and workspace management\n• Platform features and best practices\n\nWhat would you like to know more about?";
        }

        return $"Thank you for your question, {CurrentUserName}! Based on your {CurrentUserTitle} role, I recommend checking the guidance tab for role-specific information. You can also explore the knowledge base for detailed documentation. Is there anything specific I can help you with?";
    }

    private async Task RefreshRecommendations()
    {
        AIResponse = await DashboardService.GetPersonalizedAIGuidanceAsync(new PersonalizedAgentRequestDto
        {
            UserId = CurrentUserId,
            Role = CurrentUserRole
        });
        StateHasChanged();
    }

    private void ToggleAIMinimize()
    {
        IsAIChatMinimized = !IsAIChatMinimized;
        StateHasChanged();
    }

    private string GetUserInitials()
    {
        if (string.IsNullOrEmpty(CurrentUserName)) return "U";
        var parts = CurrentUserName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length >= 2)
            return $"{parts[0][0]}{parts[1][0]}".ToUpper();
        return CurrentUserName.Substring(0, Math.Min(2, CurrentUserName.Length)).ToUpper();
    }

    private async Task QuickAsk(string question)
    {
        UserMessage = question;
        await SendMessage();
    }

    // Chat Message class
    private class ChatMessage
    {
        public bool IsUser { get; set; }
        public string Content { get; set; } = "";
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }

    // ============================================
    // DEMO PIPELINE INTEGRATION
    // ============================================
    
    private DemoStatistics DemoStats { get; set; } = new();
    private List<RecentDemo> RecentDemos { get; set; } = new();

    private class DemoStatistics
    {
        public int ActiveDemos { get; set; } = 8;
        public int NewThisWeek { get; set; } = 3;
        public int PendingApprovals { get; set; } = 2;
        public int SuccessRate { get; set; } = 87;
        public int AvgCycleTime { get; set; } = 21;
    }

    private class RecentDemo
    {
        public int Id { get; set; }
        public string RequestId { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string RequestSource { get; set; } = "Internal";
        public string Status { get; set; } = string.Empty;
        public string CurrentStage { get; set; } = string.Empty;
        public int ProgressPercentage { get; set; }
    }

    private async Task LoadDemoStatisticsAsync()
    {
        try
        {
            // Call actual API endpoint with Redis caching
            var statsDto = await DemoService.GetStatisticsAsync();
            DemoStats = new DemoStatistics
            {
                ActiveDemos = statsDto.ActiveDemos,
                NewThisWeek = statsDto.NewThisWeek,
                PendingApprovals = statsDto.PendingApprovals,
                SuccessRate = statsDto.SuccessRate,
                AvgCycleTime = statsDto.AvgCycleTime
            };

            // Call actual API endpoint with Redis caching
            var recentDtos = await DemoService.GetRecentAsync(5);
            RecentDemos = recentDtos.Select(dto => new RecentDemo
            {
                Id = dto.Id,
                RequestId = $"DMO-{dto.Id:D4}",
                CustomerName = dto.CustomerName,
                RequestSource = dto.DemoType ?? "Standard",
                Status = dto.CurrentStatus,
                CurrentStage = GetStageFromStatus(dto.CurrentStatus),
                ProgressPercentage = dto.ProgressPercentage
            }).ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load demo statistics: {ex.Message}");
            // Fallback to default data
            DemoStats = new DemoStatistics { ActiveDemos = 8, NewThisWeek = 3, PendingApprovals = 2, SuccessRate = 87, AvgCycleTime = 21 };
            RecentDemos = new List<RecentDemo>();
        }
    }

    private string GetSourceBadgeClass(string source)
    {
        return source == "Internal" 
            ? "bg-primary" 
            : "bg-info";
    }

    private string GetSourceIcon(string source)
    {
        return source == "Internal" 
            ? "fa-users" 
            : "fa-user";
    }

    private string GetProgressBarClass(string status)
    {
        return status switch
        {
            "Submitted" => "bg-secondary",
            "Scheduled" => "bg-info",
            "InProgress" => "bg-warning",
            "Completed" => "bg-primary",
            "Accepted" => "bg-success",
            "POC" => "bg-success",
            "Production" => "bg-success",
            _ => "bg-secondary"
        };
    }

    private string GetStatusBadgeClass(string status)
    {
        return status switch
        {
            "Pending" or "Submitted" => "bg-secondary",
            "Approved" or "Scheduled" => "bg-info",
            "InProgress" => "bg-warning text-dark",
            "Completed" => "bg-primary",
            "Accepted" => "bg-success",
            "POC" => "bg-success",
            "Production" => "bg-success",
            "Rejected" => "bg-danger",
            _ => "bg-secondary"
        };
    }

    private string GetStageFromStatus(string status)
    {
        return status switch
        {
            "Pending" => "Awaiting Approval",
            "Approved" => "Demo Scheduled",
            "Scheduled" => "Demo Scheduled",
            "InProgress" => "Demo Execution",
            "Completed" => "Demo Completed",
            "Accepted" => "Customer Acceptance",
            "POC" => "POC Phase",
            "Production" => "Production Deployment",
            "Rejected" => "Request Rejected",
            _ => status
        };
    }

    private void ShowModuleGuide(string moduleName)
    {
        // Get base URL from configuration
        var gatewayUrl = Configuration["RemoteServices:Default:BaseUrl"] ?? "http://localhost:5000";
        
        // Navigate to the module's integration guide or swagger documentation
        var moduleUrls = new Dictionary<string, string>
        {
            ["organization"] = $"{gatewayUrl}/swagger/organization",
            ["workspace"] = $"{gatewayUrl}/swagger/workspace",
            ["ai"] = $"{gatewayUrl}/swagger/ai",
            ["document"] = $"{gatewayUrl}/swagger/document",
            ["audit"] = $"{gatewayUrl}/swagger/audit",
            ["userprofile"] = $"{gatewayUrl}/swagger/userprofile",
            ["demo"] = "/demos/requests"
        };

        if (moduleUrls.TryGetValue(moduleName.ToLower(), out var url))
        {
            NavigationManager.NavigateTo(url, forceLoad: url.StartsWith("http"));
        }
    }
}
