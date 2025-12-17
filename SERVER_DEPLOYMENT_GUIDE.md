# DG.OS Server Deployment Guide (256GB RAM)

## Quick Summary
✅ **All code committed and ready for deployment**
- Git commit: `d278441` (latest)
- Build status: Zero errors
- 59 files changed, 5360+ insertions
- Phase 1 complete: Multi-tenant branding + feature management

## What You're Deploying

### Complete Platform
- **7 Microservices** (Organization, Workspace, Document, User Profile, Audit, AI, Approval)
- **Blazor Server UI** (Modern dashboard with advanced features)
- **API Gateway** (Central routing)
- **Identity Server** (Authentication)
- **PostgreSQL Database** (Multi-tenant)

### Phase 1 Features
- Multi-tenant branding system (per-tenant logo, colors, app name)
- Feature flags (SBG, Shahin GRC, NextERP modules)
- Advanced search & filtering
- Data export (CSV)
- 5 color themes
- RTL support
- Distributed caching

## Repository Structure
```
d:\test/
├── aspnet-core/                    # Main application code
│   ├── src/                        # Source code
│   │   ├── DoganConsult.Web.Blazor/        # UI (Blazor Server)
│   │   ├── DoganConsult.Identity.HttpApi.Host/
│   │   ├── DoganConsult.Organization.HttpApi.Host/
│   │   ├── DoganConsult.Workspace.HttpApi.Host/
│   │   ├── DoganConsult.Document.HttpApi.Host/
│   │   ├── DoganConsult.UserProfile.HttpApi.Host/
│   │   ├── DoganConsult.Audit.HttpApi.Host/
│   │   ├── DoganConsult.AI.HttpApi.Host/
│   │   ├── gateway/DoganConsult.Gateway/
│   │   └── ... (other projects)
│   ├── DoganConsult.Platform.sln   # Main solution
│   └── start-services.ps1          # Service startup script
├── PHASE_1_FOUNDATION_SUMMARY.md   # Phase 1 details
├── PAGE_ENHANCEMENTS.md            # UI enhancements
├── MICROSERVICES_INVENTORY.md      # Service inventory
└── DEPLOYMENT_PACKAGE.md           # This deployment info
```

## Key Files Created (Phase 1)

### Branding System
```
DoganConsult.Workspace.Domain/Branding/BrandingProfile.cs
DoganConsult.Workspace.Application.Contracts/Branding/BrandingProfileDto.cs
DoganConsult.Workspace.Application.Contracts/Branding/IBrandingAppService.cs
DoganConsult.Workspace.Application/Branding/BrandingAppService.cs
DoganConsult.Workspace.HttpApi/Controllers/BrandingController.cs
```

### Feature Management
```
DoganConsult.Workspace.Domain.Shared/Features/DgFeatures.cs
DoganConsult.Workspace.Domain/Features/DgFeatureDefinitionProvider.cs
```

### Theme System
```
DoganConsult.Web.Blazor/Services/DgThemeService.cs
DoganConsult.Web.Blazor/wwwroot/dgTheme.js
DoganConsult.Web.Blazor/wwwroot/css/dashboard-redesign.css
DoganConsult.Web.Blazor/wwwroot/css/icon-system.css
DoganConsult.Web.Blazor/wwwroot/css/themes.css
```

### Database
```
DoganConsult.Workspace.EntityFrameworkCore/Migrations/20241217_AddBrandingProfile.cs
```

## Deployment Steps

### 1. Clone Repository
```bash
git clone <your-repo-url> /opt/doganconsult
cd /opt/doganconsult
```

### 2. Build Solution
```bash
cd aspnet-core
dotnet build DoganConsult.Platform.sln -c Release
```

### 3. Database Setup
```bash
# Apply migrations
dotnet ef database update -p DoganConsult.Workspace.EntityFrameworkCore -s DoganConsult.Workspace.HttpApi.Host

# Repeat for other services if needed
dotnet ef database update -p DoganConsult.Organization.EntityFrameworkCore -s DoganConsult.Organization.HttpApi.Host
dotnet ef database update -p DoganConsult.Document.EntityFrameworkCore -s DoganConsult.Document.HttpApi.Host
# ... etc for other services
```

### 4. Start Services
```bash
# Option A: Use provided script
./start-services.ps1

# Option B: Manual start (each in separate terminal/screen)
cd src/DoganConsult.Identity.HttpApi.Host
dotnet run --configuration Release

cd src/gateway/DoganConsult.Gateway
dotnet run --configuration Release

cd src/DoganConsult.Organization.HttpApi.Host
dotnet run --configuration Release

cd src/DoganConsult.Workspace.HttpApi.Host
dotnet run --configuration Release

cd src/DoganConsult.Document.HttpApi.Host
dotnet run --configuration Release

cd src/DoganConsult.UserProfile.HttpApi.Host
dotnet run --configuration Release

cd src/DoganConsult.Audit.HttpApi.Host
dotnet run --configuration Release

cd src/DoganConsult.AI.HttpApi.Host
dotnet run --configuration Release

cd src/DoganConsult.Web.Blazor
dotnet run --configuration Release
```

### 5. Access Application
```
https://<server-ip>:44373
```

## Configuration Files to Update

### Connection Strings
Update PostgreSQL connection strings in:
- `aspnet-core/src/DoganConsult.Identity.HttpApi.Host/appsettings.json`
- `aspnet-core/src/DoganConsult.Organization.HttpApi.Host/appsettings.json`
- `aspnet-core/src/DoganConsult.Workspace.HttpApi.Host/appsettings.json`
- `aspnet-core/src/DoganConsult.Document.HttpApi.Host/appsettings.json`
- `aspnet-core/src/DoganConsult.UserProfile.HttpApi.Host/appsettings.json`
- `aspnet-core/src/DoganConsult.Audit.HttpApi.Host/appsettings.json`
- `aspnet-core/src/DoganConsult.AI.HttpApi.Host/appsettings.json`

### Example Connection String
```json
{
  "ConnectionStrings": {
    "Default": "Server=<db-server>;Port=5432;Database=doganconsult;User Id=<user>;Password=<password>;"
  }
}
```

## API Endpoints

### Branding API
```
GET  /api/dg/branding/current     - Get current tenant branding
PUT  /api/dg/branding/update      - Update branding
```

### Organizations
```
GET    /api/organization/organizations
POST   /api/organization/organizations
GET    /api/organization/organizations/{id}
PUT    /api/organization/organizations/{id}
DELETE /api/organization/organizations/{id}
```

### Workspaces
```
GET    /api/workspace/workspaces
POST   /api/workspace/workspaces
GET    /api/workspace/workspaces/{id}
PUT    /api/workspace/workspaces/{id}
DELETE /api/workspace/workspaces/{id}
```

### Documents
```
GET    /api/document/documents
POST   /api/document/documents
GET    /api/document/documents/{id}
PUT    /api/document/documents/{id}
DELETE /api/document/documents/{id}
```

### User Profiles
```
GET    /api/userprofile/user-profiles
POST   /api/userprofile/user-profiles
GET    /api/userprofile/user-profiles/{id}
PUT    /api/userprofile/user-profiles/{id}
DELETE /api/userprofile/user-profiles/{id}
```

### Audit Logs
```
GET    /api/audit/audit-logs
GET    /api/audit/audit-logs/{id}
```

### AI Service
```
POST   /api/ai/chat
```

## Database Schema

### New Table: BrandingProfiles
```sql
CREATE TABLE "AppBrandingProfiles" (
    "Id" uuid PRIMARY KEY,
    "TenantId" uuid,
    "AppDisplayName" varchar(128) NOT NULL,
    "ProductName" varchar(128),
    "LogoUrl" varchar(512),
    "FaviconUrl" varchar(512),
    "PrimaryColor" varchar(32) NOT NULL,
    "AccentColor" varchar(32) NOT NULL,
    "DefaultLanguage" varchar(10) NOT NULL,
    "IsRtl" boolean NOT NULL,
    "HomeRoute" varchar(256) NOT NULL,
    "CreationTime" timestamp NOT NULL,
    "CreatorId" uuid,
    "LastModificationTime" timestamp,
    "LastModifierId" uuid,
    "IsDeleted" boolean NOT NULL DEFAULT false,
    "DeleterId" uuid,
    "DeletionTime" timestamp
);

CREATE INDEX "IX_AppBrandingProfiles_TenantId" ON "AppBrandingProfiles" ("TenantId");
```

## Feature Flags

### Available Features
```
DG.Modules.SBG              - Saudi Business Gate
DG.Modules.ShahinGrc        - Shahin GRC
DG.Modules.NextERP          - NextERP

Sub-features:
DG.Modules.ShahinGrc.Risk
DG.Modules.ShahinGrc.Controls
DG.Modules.SBG.Procurement
DG.Modules.SBG.Contracts
```

### Enable Feature for Tenant
Features are managed via ABP Feature Management UI in the admin panel.

## Performance Tuning (256GB RAM)

### Recommended Settings
```json
{
  "Caching": {
    "RedisConnection": "redis-server:6379",
    "DefaultSlidingExpiration": 3600
  },
  "Database": {
    "ConnectionPoolSize": 100,
    "CommandTimeout": 30
  },
  "Blazor": {
    "CircuitOptions": {
      "DisconnectedCircuitMaxRetained": 100,
      "DisconnectedCircuitRetentionPeriod": "00:03:00"
    }
  }
}
```

### Redis Setup (Optional but Recommended)
```bash
docker run -d -p 6379:6379 redis:latest
```

## Monitoring

### Health Check Endpoints
```
https://localhost:44373/health
https://localhost:5001/health
https://localhost:44346/health
```

### Logs Location
```
aspnet-core/src/*/Logs/
```

## Troubleshooting

### Port Already in Use
```bash
# Find process using port
lsof -i :44373

# Kill process
kill -9 <PID>
```

### Database Connection Failed
- Verify PostgreSQL is running
- Check connection string
- Verify credentials
- Check firewall rules

### Service Won't Start
- Check logs in `Logs/` directory
- Verify all dependencies are installed
- Ensure .NET 10.0 SDK is installed

## Backup & Recovery

### Database Backup
```bash
pg_dump -U <user> -d doganconsult > backup.sql
```

### Database Restore
```bash
psql -U <user> -d doganconsult < backup.sql
```

## Next Steps (Phase 2)

1. **Plugin Architecture**
   - Create `IDgModuleUiContributor` interface
   - Implement menu/widget contribution pattern
   - Feature-gate all contributions

2. **Shell Gating**
   - Hide menu items for disabled modules
   - Block page access for disabled modules
   - Show "Module not enabled" message

3. **Tenant Admin UI**
   - Branding editor
   - Feature toggles
   - Role/permission management

4. **Sample Module**
   - Create SBG.Sample module
   - Demonstrate feature-gated menu
   - Demonstrate feature-gated widget

## Support Resources

- `PHASE_1_FOUNDATION_SUMMARY.md` - Technical details
- `PAGE_ENHANCEMENTS.md` - UI features
- `MICROSERVICES_INVENTORY.md` - Service details
- `DEPLOYMENT_PACKAGE.md` - Deployment info

## Git Information

**Latest Commit**: `d278441`
**Message**: "Add comprehensive deployment package documentation"
**Previous**: `13ea583` - "Phase 1: DG.Foundation Multi-Tenant Platform - Complete Implementation"

**Total Changes in Phase 1**:
- 59 files changed
- 5360+ insertions
- 403 deletions

## Deployment Checklist

- [ ] Repository cloned
- [ ] Build successful
- [ ] Database migrations applied
- [ ] Connection strings configured
- [ ] Redis configured (optional)
- [ ] All services started
- [ ] UI accessible at https://<server-ip>:44373
- [ ] API endpoints responding
- [ ] Branding API working
- [ ] Feature flags visible in admin
- [ ] Logs monitored
- [ ] Backups configured

## Contact & Support

For issues or questions:
1. Check logs in `Logs/` directory
2. Review troubleshooting section
3. Check documentation files
4. Verify configuration files

---

**Status**: ✅ READY FOR PRODUCTION
**Date**: December 17, 2025
**Version**: Phase 1 Complete
**Build**: d278441
**RAM Available**: 256GB (More than sufficient)
