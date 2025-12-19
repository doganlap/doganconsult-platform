using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DoganConsult.AI.Application.Contracts.Services;
using DoganConsult.AI.Permissions;
using DoganConsult.Web.Blazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Volo.Abp.Authorization.Permissions;

namespace DoganConsult.Web.Blazor.Components.Pages;

public partial class AIChat : ComponentBase
{
    [Inject] private AIService AIService { get; set; } = default!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
    [Inject] private IPermissionChecker PermissionChecker { get; set; } = default!;
    
    // Permission checks
    private bool CanUseAI = false;

    private ElementReference chatContainer;
    private List<ChatMessage> ChatMessages = new();
    private string CurrentMessage = string.Empty;
    private string? CurrentThreadId = null;
    private bool IsThinking = false;
    private int EstimatedTokens = 0;

    // Modal states
    private bool ShowDocumentAnalysisModal = false;
    private bool ShowComplianceModal = false;
    private bool ShowRiskModal = false;
    private bool AnalyzeLoading = false;

    // Document Analysis
    private string DocumentAnalysisContent = string.Empty;
    private string DocumentAnalysisType = "policy";
    private DocumentAnalysisResultDto? DocumentAnalysisResult = null;

    // Compliance Check
    private string ComplianceOrgType = "financial";
    private string ComplianceSector = string.Empty;
    private string ComplianceData = string.Empty;
    private ComplianceCheckResultDto? ComplianceResult = null;

    // Risk Assessment
    private string RiskCategory = "operational";
    private string RiskOrgContext = string.Empty;
    private string RiskDescription = string.Empty;
    private RiskAssessmentResultDto? RiskResult = null;

    protected override async Task OnInitializedAsync()
    {
        await LoadPermissions();
    }

    private async Task LoadPermissions()
    {
        CanUseAI = await PermissionChecker.IsGrantedAsync(AIPermissions.AIRequests.Create);
    }

    private async Task SendMessage()
    {
        if (string.IsNullOrWhiteSpace(CurrentMessage)) return;
        if (!CanUseAI) return;

        var userMessage = CurrentMessage;
        CurrentMessage = string.Empty;
        
        ChatMessages.Add(new ChatMessage
        {
            Content = userMessage,
            IsUser = true,
            Timestamp = DateTime.Now
        });
        
        IsThinking = true;
        StateHasChanged();
        await ScrollToBottom();

        try
        {
            ChatResponseDto? response;
            
            if (!string.IsNullOrEmpty(CurrentThreadId))
            {
                response = await AIService.ContinueConversationAsync(CurrentThreadId, userMessage);
            }
            else
            {
                response = await AIService.ChatAsync(new ChatRequestDto
                {
                    Message = userMessage,
                    ThreadId = CurrentThreadId
                });
            }

            if (response != null)
            {
                CurrentThreadId = response.ThreadId;
                ChatMessages.Add(new ChatMessage
                {
                    Content = response.Response,
                    IsUser = false,
                    Timestamp = response.Timestamp,
                    ResponseTimeMs = response.ResponseTimeMs
                });
                EstimatedTokens += EstimateTokens(userMessage) + EstimateTokens(response.Response);
            }
        }
        catch (Exception ex)
        {
            ChatMessages.Add(new ChatMessage
            {
                Content = $"Sorry, I encountered an error: {ex.Message}. Please try again.",
                IsUser = false,
                Timestamp = DateTime.Now
            });
        }
        finally
        {
            IsThinking = false;
            StateHasChanged();
            await ScrollToBottom();
        }
    }

    private async Task SendQuickMessage(string message)
    {
        CurrentMessage = message;
        await SendMessage();
    }

    private async Task HandleKeyPress(KeyboardEventArgs e)
    {
        if (e.Key == "Enter" && e.CtrlKey)
        {
            await SendMessage();
        }
    }

    private void ClearChat()
    {
        ChatMessages.Clear();
        CurrentThreadId = null;
        EstimatedTokens = 0;
        StateHasChanged();
    }

    private async Task StartNewThread()
    {
        try
        {
            ClearChat();
            CurrentThreadId = await AIService.CreateConversationThreadAsync(Guid.NewGuid());
            await ShowAlert("New conversation started!");
        }
        catch (Exception ex)
        {
            await ShowAlert($"Error starting new conversation: {ex.Message}");
        }
    }

    private async Task ScrollToBottom()
    {
        await Task.Delay(50);
        await JSRuntime.InvokeVoidAsync("eval", "document.querySelector('.chat-container').scrollTop = document.querySelector('.chat-container').scrollHeight");
    }

    private void OpenDocumentAnalysis()
    {
        DocumentAnalysisContent = string.Empty;
        DocumentAnalysisResult = null;
        ShowDocumentAnalysisModal = true;
    }

    private void OpenComplianceCheck()
    {
        ComplianceData = string.Empty;
        ComplianceResult = null;
        ShowComplianceModal = true;
    }

    private void OpenRiskAssessment()
    {
        RiskDescription = string.Empty;
        RiskResult = null;
        ShowRiskModal = true;
    }

    private void OpenAuditSummary()
    {
        CurrentMessage = "Please help me generate an audit summary for my organization.";
        StateHasChanged();
    }

    private async Task RunDocumentAnalysis()
    {
        if (string.IsNullOrWhiteSpace(DocumentAnalysisContent)) return;

        AnalyzeLoading = true;
        try
        {
            DocumentAnalysisResult = await AIService.AnalyzeDocumentAsync(new DocumentAnalysisRequestDto
            {
                Content = DocumentAnalysisContent,
                DocumentType = DocumentAnalysisType,
                AnalysisTypes = new List<string> { "summary", "compliance", "risk" }
            });
        }
        catch (Exception ex)
        {
            await ShowAlert($"Error analyzing document: {ex.Message}");
        }
        finally
        {
            AnalyzeLoading = false;
            StateHasChanged();
        }
    }

    private async Task RunComplianceCheck()
    {
        if (string.IsNullOrWhiteSpace(ComplianceData)) return;

        AnalyzeLoading = true;
        try
        {
            ComplianceResult = await AIService.CheckComplianceAsync(new ComplianceCheckRequestDto
            {
                OrganizationType = ComplianceOrgType,
                Sector = ComplianceSector,
                DataToCheck = ComplianceData,
                ApplicableRegulations = new List<string> { "GDPR", "SOX", "ISO27001" }
            });
        }
        catch (Exception ex)
        {
            await ShowAlert($"Error checking compliance: {ex.Message}");
        }
        finally
        {
            AnalyzeLoading = false;
            StateHasChanged();
        }
    }

    private async Task RunRiskAssessment()
    {
        if (string.IsNullOrWhiteSpace(RiskDescription)) return;

        AnalyzeLoading = true;
        try
        {
            RiskResult = await AIService.AssessRiskAsync(new RiskAssessmentRequestDto
            {
                RiskCategory = RiskCategory,
                Description = RiskDescription,
                OrganizationContext = RiskOrgContext,
                RiskFactors = new Dictionary<string, decimal>()
            });
        }
        catch (Exception ex)
        {
            await ShowAlert($"Error assessing risk: {ex.Message}");
        }
        finally
        {
            AnalyzeLoading = false;
            StateHasChanged();
        }
    }

    private static string GetRiskAlertClass(string riskLevel) => riskLevel?.ToLower() switch
    {
        "low" => "alert-success",
        "medium" => "alert-warning",
        "high" => "alert-danger",
        "critical" => "alert-danger",
        _ => "alert-info"
    };

    private static int EstimateTokens(string text)
    {
        return (int)Math.Ceiling(text.Length / 4.0);
    }

    private async Task ShowAlert(string message)
    {
        await JSRuntime.InvokeVoidAsync("alert", message);
    }

    private class ChatMessage
    {
        public string Content { get; set; } = string.Empty;
        public bool IsUser { get; set; }
        public DateTime Timestamp { get; set; }
        public int ResponseTimeMs { get; set; }
    }
}
