using System;
using System.Threading.Tasks;
using DoganConsult.Audit.Approvals;
using DoganConsult.Audit.Workflow;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Emailing;
using Volo.Abp.TextTemplating;

namespace DoganConsult.Audit.Notifications;

public interface IApprovalNotificationService
{
    Task SendApprovalRequestNotificationAsync(ApprovalRequest request);
    Task SendApprovalDecisionNotificationAsync(ApprovalRequest request, bool approved);
    Task SendApprovalReassignedNotificationAsync(ApprovalRequest request, string previousApproverName);
    Task SendApprovalExpiringNotificationAsync(ApprovalRequest request);
}

public class ApprovalNotificationService : IApprovalNotificationService, ITransientDependency
{
    private readonly IEmailSender _emailSender;
    private readonly ILogger<ApprovalNotificationService> _logger;

    public ApprovalNotificationService(
        IEmailSender emailSender,
        ILogger<ApprovalNotificationService> logger)
    {
        _emailSender = emailSender;
        _logger = logger;
    }

    public async Task SendApprovalRequestNotificationAsync(ApprovalRequest request)
    {
        try
        {
            // In a real implementation, you would fetch the approver's email
            // For now, we'll log the notification
            var subject = $"[Action Required] New Approval Request: {request.RequestNumber}";
            var body = GenerateApprovalRequestEmailBody(request);

            _logger.LogInformation(
                "Approval notification email would be sent. " +
                "Subject: {Subject}, RequestNumber: {RequestNumber}, " +
                "EntityType: {EntityType}, AssignedApprover: {Approver}",
                subject, request.RequestNumber, request.EntityType, request.AssignedApproverName ?? "Any Approver");

            // Uncomment when email is configured:
            // if (!string.IsNullOrEmpty(approverEmail))
            // {
            //     await _emailSender.SendAsync(approverEmail, subject, body, isBodyHtml: true);
            // }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send approval request notification for {RequestNumber}", request.RequestNumber);
        }
    }

    public async Task SendApprovalDecisionNotificationAsync(ApprovalRequest request, bool approved)
    {
        try
        {
            var decision = approved ? "Approved" : "Rejected";
            var subject = $"[{decision}] Approval Request: {request.RequestNumber}";
            var body = GenerateDecisionEmailBody(request, approved);

            _logger.LogInformation(
                "Decision notification email would be sent. " +
                "Subject: {Subject}, RequestNumber: {RequestNumber}, " +
                "Decision: {Decision}, Requester: {Requester}",
                subject, request.RequestNumber, decision, request.RequesterName);

            // Uncomment when email is configured:
            // if (!string.IsNullOrEmpty(request.RequesterEmail))
            // {
            //     await _emailSender.SendAsync(request.RequesterEmail, subject, body, isBodyHtml: true);
            // }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send decision notification for {RequestNumber}", request.RequestNumber);
        }
    }

    public async Task SendApprovalReassignedNotificationAsync(ApprovalRequest request, string previousApproverName)
    {
        try
        {
            var subject = $"[Reassigned] Approval Request: {request.RequestNumber}";
            var body = $@"
                <h2>Approval Request Reassigned</h2>
                <p>The following approval request has been reassigned to you:</p>
                <ul>
                    <li><strong>Request Number:</strong> {request.RequestNumber}</li>
                    <li><strong>Entity:</strong> {request.EntityType} - {request.EntityName}</li>
                    <li><strong>Action:</strong> {request.RequestedAction}</li>
                    <li><strong>Previous Approver:</strong> {previousApproverName ?? "Unassigned"}</li>
                    <li><strong>Requested By:</strong> {request.RequesterName}</li>
                    <li><strong>Priority:</strong> {request.Priority}</li>
                </ul>
                <p>Please review and take action on this request.</p>
            ";

            _logger.LogInformation(
                "Reassignment notification email would be sent. " +
                "Subject: {Subject}, RequestNumber: {RequestNumber}, " +
                "NewApprover: {NewApprover}",
                subject, request.RequestNumber, request.AssignedApproverName);

            // Uncomment when email is configured
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send reassignment notification for {RequestNumber}", request.RequestNumber);
        }
    }

    public async Task SendApprovalExpiringNotificationAsync(ApprovalRequest request)
    {
        try
        {
            var subject = $"[Expiring Soon] Approval Request: {request.RequestNumber}";
            var body = $@"
                <h2>Approval Request Expiring Soon</h2>
                <p>The following approval request is about to expire:</p>
                <ul>
                    <li><strong>Request Number:</strong> {request.RequestNumber}</li>
                    <li><strong>Entity:</strong> {request.EntityType} - {request.EntityName}</li>
                    <li><strong>Expires At:</strong> {request.ExpiresAt:yyyy-MM-dd HH:mm}</li>
                    <li><strong>Requested By:</strong> {request.RequesterName}</li>
                </ul>
                <p>Please take action before the request expires.</p>
            ";

            _logger.LogInformation(
                "Expiration warning email would be sent. " +
                "Subject: {Subject}, RequestNumber: {RequestNumber}, " +
                "ExpiresAt: {ExpiresAt}",
                subject, request.RequestNumber, request.ExpiresAt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send expiration notification for {RequestNumber}", request.RequestNumber);
        }
    }

    private string GenerateApprovalRequestEmailBody(ApprovalRequest request)
    {
        return $@"
            <!DOCTYPE html>
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                    .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                    .header {{ background-color: #007bff; color: white; padding: 20px; text-align: center; }}
                    .content {{ padding: 20px; background-color: #f9f9f9; }}
                    .details {{ background-color: white; padding: 15px; border-radius: 5px; margin: 15px 0; }}
                    .details dl {{ margin: 0; }}
                    .details dt {{ font-weight: bold; color: #555; }}
                    .details dd {{ margin: 0 0 10px 0; }}
                    .priority-high {{ color: #dc3545; font-weight: bold; }}
                    .priority-urgent {{ color: #dc3545; font-weight: bold; background-color: #ffe0e0; padding: 2px 8px; }}
                    .button {{ display: inline-block; padding: 12px 24px; background-color: #007bff; color: white; text-decoration: none; border-radius: 5px; margin: 10px 5px; }}
                    .button.approve {{ background-color: #28a745; }}
                    .button.reject {{ background-color: #dc3545; }}
                    .footer {{ text-align: center; color: #666; font-size: 12px; padding: 20px; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h1>New Approval Request</h1>
                    </div>
                    <div class='content'>
                        <p>You have received a new approval request that requires your attention.</p>
                        
                        <div class='details'>
                            <dl>
                                <dt>Request Number</dt>
                                <dd>{request.RequestNumber}</dd>
                                
                                <dt>Entity Type</dt>
                                <dd>{request.EntityType}</dd>
                                
                                <dt>Entity Name</dt>
                                <dd>{request.EntityName}</dd>
                                
                                <dt>Requested Action</dt>
                                <dd>{request.RequestedAction}</dd>
                                
                                <dt>Requested By</dt>
                                <dd>{request.RequesterName}</dd>
                                
                                <dt>Priority</dt>
                                <dd class='{(request.Priority >= ApprovalPriority.High ? "priority-high" : "")}'>{request.Priority}</dd>
                                
                                {(request.ExpiresAt.HasValue ? $"<dt>Expires At</dt><dd>{request.ExpiresAt:yyyy-MM-dd HH:mm}</dd>" : "")}
                                
                                {(!string.IsNullOrEmpty(request.RequestReason) ? $"<dt>Reason</dt><dd>{request.RequestReason}</dd>" : "")}
                            </dl>
                        </div>
                        
                        <p>Please log in to the approval portal to review and process this request.</p>
                    </div>
                    <div class='footer'>
                        <p>This is an automated notification from the DoganConsult Approval System.</p>
                        <p>Please do not reply to this email.</p>
                    </div>
                </div>
            </body>
            </html>
        ";
    }

    private string GenerateDecisionEmailBody(ApprovalRequest request, bool approved)
    {
        var statusColor = approved ? "#28a745" : "#dc3545";
        var statusText = approved ? "Approved" : "Rejected";

        return $@"
            <!DOCTYPE html>
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                    .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                    .header {{ background-color: {statusColor}; color: white; padding: 20px; text-align: center; }}
                    .content {{ padding: 20px; background-color: #f9f9f9; }}
                    .details {{ background-color: white; padding: 15px; border-radius: 5px; margin: 15px 0; }}
                    .details dl {{ margin: 0; }}
                    .details dt {{ font-weight: bold; color: #555; }}
                    .details dd {{ margin: 0 0 10px 0; }}
                    .comments {{ background-color: #e9ecef; padding: 15px; border-left: 4px solid {statusColor}; margin: 15px 0; }}
                    .footer {{ text-align: center; color: #666; font-size: 12px; padding: 20px; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h1>Request {statusText}</h1>
                    </div>
                    <div class='content'>
                        <p>Your approval request has been <strong>{statusText.ToLower()}</strong>.</p>
                        
                        <div class='details'>
                            <dl>
                                <dt>Request Number</dt>
                                <dd>{request.RequestNumber}</dd>
                                
                                <dt>Entity</dt>
                                <dd>{request.EntityType} - {request.EntityName}</dd>
                                
                                <dt>Requested Action</dt>
                                <dd>{request.RequestedAction}</dd>
                                
                                <dt>{statusText} By</dt>
                                <dd>{request.ApprovedByName}</dd>
                                
                                <dt>Decision Date</dt>
                                <dd>{request.ApprovedAt:yyyy-MM-dd HH:mm}</dd>
                            </dl>
                        </div>
                        
                        {(!string.IsNullOrEmpty(request.ApprovalComments) ? $@"
                        <div class='comments'>
                            <strong>Comments from Approver:</strong>
                            <p>{request.ApprovalComments}</p>
                        </div>
                        " : "")}
                    </div>
                    <div class='footer'>
                        <p>This is an automated notification from the DoganConsult Approval System.</p>
                    </div>
                </div>
            </body>
            </html>
        ";
    }
}
