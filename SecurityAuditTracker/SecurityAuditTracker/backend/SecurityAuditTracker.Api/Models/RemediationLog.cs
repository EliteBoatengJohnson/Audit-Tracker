namespace SecurityAuditTracker.Api.Models;

public class RemediationLog
{
    public int Id { get; set; }

    public int FindingId { get; set; }
    public Finding? Finding { get; set; }

    public string Comment { get; set; } = string.Empty;
    public FindingStatus? StatusChangedTo { get; set; }

    public int ChangedById { get; set; }
    public User? ChangedBy { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
