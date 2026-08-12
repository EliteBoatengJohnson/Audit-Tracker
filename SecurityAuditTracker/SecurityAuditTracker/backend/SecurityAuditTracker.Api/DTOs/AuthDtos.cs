using SecurityAuditTracker.Api.Models;

namespace SecurityAuditTracker.Api.DTOs;

public record RegisterRequest(string Name, string Email, string Password, UserRole Role);

public record LoginRequest(string Email, string Password);

public record AuthResponse(string Token, string Name, string Email, string Role, DateTime ExpiresAt);
