using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecurityAuditTracker.Api.Data;
using SecurityAuditTracker.Api.DTOs;

namespace SecurityAuditTracker.Api.Controllers;

[ApiController]
[Route("api/findings/{findingId}/remediation")]
[Authorize]
public class RemediationController : ControllerBase
{
    private readonly AppDbContext _db;

    public RemediationController(AppDbContext db)
    {
        _db = db;
    }

    private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User.FindFirstValue("sub")!);

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RemediationLogResponse>>> GetLogs(int findingId)
    {
        var logs = await _db.RemediationLogs
            .Include(r => r.ChangedBy)
            .Where(r => r.FindingId == findingId)
            .OrderBy(r => r.Timestamp)
            .ToListAsync();

        return Ok(logs.Select(l => new RemediationLogResponse(
            l.Id, l.Comment, l.StatusChangedTo?.ToString(), l.ChangedBy?.Name ?? "Unknown", l.Timestamp)));
    }

    [HttpPost]
    public async Task<ActionResult<RemediationLogResponse>> AddLog(int findingId, AddRemediationLogRequest req)
    {
        var finding = await _db.Findings.FindAsync(findingId);
        if (finding is null) return NotFound("Finding not found.");

        var log = new Models.RemediationLog
        {
            FindingId = findingId,
            Comment = req.Comment,
            StatusChangedTo = req.StatusChangedTo,
            ChangedById = CurrentUserId
        };

        _db.RemediationLogs.Add(log);

        // Keep the finding's status in sync with the latest remediation update
        if (req.StatusChangedTo.HasValue)
        {
            finding.Status = req.StatusChangedTo.Value;
            if (req.StatusChangedTo.Value == Models.FindingStatus.Resolved)
                finding.ResolvedAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync();
        await _db.Entry(log).Reference(l => l.ChangedBy).LoadAsync();

        return Ok(new RemediationLogResponse(
            log.Id, log.Comment, log.StatusChangedTo?.ToString(), log.ChangedBy?.Name ?? "Unknown", log.Timestamp));
    }
}
