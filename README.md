# Security Audit Findings Tracker

A tool for logging, prioritising, assigning, and tracking remediation of IT security/audit findings, mapped to a compliance framework (ISO 27001 / PCI DSS v4.0 / NIST SP 800-53 / BoG Guidelines).

Built for CSCD602 Advanced Software Engineering — 48-hour individual project exam.

## Stack
- **Backend:** ASP.NET Core Web API (.NET 10), EF Core, PostgreSQL, JWT auth, BCrypt password hashing
- **Frontend:** Vue 3 + Vite, Pinia, Vue Router, Axios

## Prerequisites
- .NET 10 SDK
- Node.js 18+
- PostgreSQL running locally (or a connection string to a hosted instance)

## 1. Backend Setup

```bash
cd backend/SecurityAuditTracker.Api

# Update the connection string in appsettings.json (or use appsettings.Development.json / user-secrets)
# "DefaultConnection": "Host=localhost;Port=5432;Database=audit_tracker;Username=postgres;Password=postgres"

dotnet restore
dotnet tool install --global dotnet-ef   # if not already installed
dotnet ef migrations add InitialCreate
dotnet run
```

The API applies pending migrations automatically on startup. Swagger UI is available at `https://localhost:5001/swagger` in development.

**Important:** Change the `Jwt:Key` in `appsettings.json` to a real random secret before deploying — the placeholder value is not safe for production.

## 2. Frontend Setup

```bash
cd frontend
npm install
npm run dev
```

Runs at `http://localhost:5173`. The `.env` file points to `https://localhost:5001/api` — update `VITE_API_BASE_URL` if your backend runs elsewhere or once deployed.

## 3. Roles

| Role | Can do |
|---|---|
| Auditor | Create/edit findings, add remediation updates |
| Owner | View findings, add remediation updates on assigned findings |
| Manager | Everything Auditor can do, plus delete findings, full dashboard visibility |

Register a user via `/register` and pick a role to test each permission level. In a real deployment, role assignment would be admin-controlled rather than self-selected at signup — noted as a known limitation / technical debt item.

## 4. Core Features Implemented
- JWT-based authentication and role-based authorization (server-enforced, not just UI-hidden)
- Create, view, edit, delete findings with severity, framework reference, control reference, owner, and due date
- Remediation log / audit trail per finding — every update is timestamped and attributed to a user
- Automatic overdue detection based on due date and status
- Dashboard summary: counts by status, severity breakdown, overdue count
- Filtering findings by status, severity, and owner

## 5. Known Technical Debt (see full plan in project documentation)
- SQLite/dev-grade Postgres instance rather than a managed production database
- No email/notification delivery for overdue findings (would need a real SMTP/queue integration)
- Role assigned at self-registration rather than admin-provisioned
- Minimal automated test coverage — manual + targeted unit tests only, given the 48-hour window
- No CSV export yet (documented as a "should-have" not implemented in this cycle)

## 6. Deployment
- **Backend:** deploy to Render/Azure App Service; provision a managed Postgres instance (Render Postgres, Azure Database for PostgreSQL, or Supabase) and set `ConnectionStrings:DefaultConnection` via environment variable/secret
- **Frontend:** deploy to Vercel/Netlify; set `VITE_API_BASE_URL` to the deployed backend URL as a build-time environment variable
