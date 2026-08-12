namespace SecurityAuditTracker.Api.Models;

public enum UserRole
{
    Auditor,
    Owner,
    Manager
}

public enum Severity
{
    Low,
    Medium,
    High,
    Critical
}

public enum FindingStatus
{
    Open,
    InProgress,
    Resolved,
    AcceptedRisk
}

public enum FrameworkReference
{
    ISO27001,
    PCIDSSv4,
    NISTSP80053,
    BoGGuidelines,
    Other
}
