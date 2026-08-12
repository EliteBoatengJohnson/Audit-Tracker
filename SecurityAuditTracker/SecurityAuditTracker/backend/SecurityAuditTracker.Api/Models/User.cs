namespace SecurityAuditTracker.Api.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Finding> OwnedFindings { get; set; } = new List<Finding>();
    public ICollection<Finding> CreatedFindings { get; set; } = new List<Finding>();
    public ICollection<RemediationLog> RemediationLogs { get; set; } = new List<RemediationLog>();
}
