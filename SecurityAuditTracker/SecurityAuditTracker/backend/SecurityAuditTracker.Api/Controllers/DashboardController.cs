using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecurityAuditTracker.Api.Data;
using SecurityAuditTracker.Api.DTOs;
using SecurityAuditTracker.Api.Models;

namespace SecurityAuditTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly AppDbContext _db;

    public DashboardController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet("summary")]
    public async Task<ActionResult<DashboardSummaryResponse>> Summary()
    {
        var findings = await _db.Findings.ToListAsync();

        var summary = new DashboardSummaryResponse(
            TotalFindings: findings.Count,
            OpenCount: findings.Count(f => f.Status == FindingStatus.Open),
            InProgressCount: findings.Count(f => f.Status == FindingStatus.InProgress),
            ResolvedCount: findings.Count(f => f.Status == FindingStatus.Resolved),
            AcceptedRiskCount: findings.Count(f => f.Status == FindingStatus.AcceptedRisk),
            OverdueCount: findings.Count(f => f.IsOverdue),
            BySeverity: findings
                .GroupBy(f => f.Severity.ToString())
                .ToDictionary(g => g.Key, g => g.Count())
        );

        return Ok(summary);
    }
}
