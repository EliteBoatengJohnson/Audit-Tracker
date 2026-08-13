using System.Security.Claims;
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
public class FindingsController : ControllerBase
{
    private readonly AppDbContext _db;

    public FindingsController(AppDbContext db)
    {
        _db = db;
    }

    private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User.FindFirstValue("sub")!);

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FindingResponse>>> GetAll(
        [FromQuery] string? status, [FromQuery] string? severity, [FromQuery] int? ownerId)
    {
        var query = _db.Findings
            .Include(f => f.Owner)
            .Include(f => f.CreatedBy)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<FindingStatus>(status, true, out var st))
            query = query.Where(f => f.Status == st);

        if (!string.IsNullOrEmpty(severity) && Enum.TryParse<Severity>(severity, true, out var sev))
            query = query.Where(f => f.Severity == sev);

        if (ownerId.HasValue)
            query = query.Where(f => f.OwnerId == ownerId);

        var findings = await query.OrderByDescending(f => f.CreatedAt).ToListAsync();

        return Ok(findings.Select(ToResponse));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<FindingResponse>> GetById(int id)
    {
        var finding = await _db.Findings
            .Include(f => f.Owner)
            .Include(f => f.CreatedBy)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (finding is null) return NotFound();
        return Ok(ToResponse(finding));
    }

    [HttpPost]
    [Authorize(Roles = "Auditor,Manager")]
    public async Task<ActionResult<FindingResponse>> Create(CreateFindingRequest req)
    {
        var finding = new Finding
        {
            Title = req.Title,
            Description = req.Description,
            Severity = req.Severity,
            FrameworkReference = req.FrameworkReference,
            ControlReference = req.ControlReference,
            OwnerId = req.OwnerId,
            DueDate = req.DueDate.HasValue ? DateTime.SpecifyKind(req.DueDate.Value, DateTimeKind.Utc) : null,
            CreatedById = CurrentUserId
        };

        _db.Findings.Add(finding);
        await _db.SaveChangesAsync();
        await _db.Entry(finding).Reference(f => f.Owner).LoadAsync();
        await _db.Entry(finding).Reference(f => f.CreatedBy).LoadAsync();

        return CreatedAtAction(nameof(GetById), new { id = finding.Id }, ToResponse(finding));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Auditor,Manager")]
    public async Task<IActionResult> Update(int id, UpdateFindingRequest req)
    {
        var finding = await _db.Findings.FindAsync(id);
        if (finding is null) return NotFound();

        finding.Title = req.Title;
        finding.Description = req.Description;
        finding.Severity = req.Severity;
        finding.FrameworkReference = req.FrameworkReference;
        finding.ControlReference = req.ControlReference;
        finding.OwnerId = req.OwnerId;
        finding.DueDate = req.DueDate.HasValue ? DateTime.SpecifyKind(req.DueDate.Value, DateTimeKind.Utc) : null;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> Delete(int id)
    {
        var finding = await _db.Findings.FindAsync(id);
        if (finding is null) return NotFound();

        _db.Findings.Remove(finding);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private static FindingResponse ToResponse(Finding f) => new(
        f.Id, f.Title, f.Description, f.Severity.ToString(), f.FrameworkReference.ToString(),
        f.ControlReference, f.Status.ToString(), f.Owner?.Name, f.CreatedBy?.Name ?? "Unknown",
        f.DueDate, f.IsOverdue, f.CreatedAt);
}
