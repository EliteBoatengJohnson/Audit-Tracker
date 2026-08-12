namespace SecurityAuditTracker.Api.Models;

public class Finding
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Severity Severity { get; set; }
    public FrameworkReference FrameworkReference { get; set; }
    public string? ControlReference { get; set; } // e.g. "A.9.2.3" or "Req 8.3"
    public FindingStatus Status { get; set; } = FindingStatus.Open;

    public int? OwnerId { get; set; }
    public User? Owner { get; set; }

    public int CreatedById { get; set; }
    public User? CreatedBy { get; set; }

    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedAt { get; set; }

    public ICollection<RemediationLog> RemediationLogs { get; set; } = new List<RemediationLog>();

    public bool IsOverdue => DueDate.HasValue
        && Status != FindingStatus.Resolved
        && Status != FindingStatus.AcceptedRisk
        && DueDate.Value < DateTime.UtcNow;
}
