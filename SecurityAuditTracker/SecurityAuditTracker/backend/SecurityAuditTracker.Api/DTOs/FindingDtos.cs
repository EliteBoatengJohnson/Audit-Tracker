using SecurityAuditTracker.Api.Models;

namespace SecurityAuditTracker.Api.DTOs;

public record CreateFindingRequest(
    string Title,
    string Description,
    Severity Severity,
    FrameworkReference FrameworkReference,
    string? ControlReference,
    int? OwnerId,
    DateTime? DueDate);

public record UpdateFindingRequest(
    string Title,
    string Description,
    Severity Severity,
    FrameworkReference FrameworkReference,
    string? ControlReference,
    int? OwnerId,
    DateTime? DueDate);

public record FindingResponse(
    int Id,
    string Title,
    string Description,
    string Severity,
    string FrameworkReference,
    string? ControlReference,
    string Status,
    string? OwnerName,
    string CreatedByName,
    DateTime? DueDate,
    bool IsOverdue,
    DateTime CreatedAt);

public record AddRemediationLogRequest(string Comment, FindingStatus? StatusChangedTo);

public record RemediationLogResponse(
    int Id,
    string Comment,
    string? StatusChangedTo,
    string ChangedByName,
    DateTime Timestamp);

public record DashboardSummaryResponse(
    int TotalFindings,
    int OpenCount,
    int InProgressCount,
    int ResolvedCount,
    int AcceptedRiskCount,
    int OverdueCount,
    Dictionary<string, int> BySeverity);
