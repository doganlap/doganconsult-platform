# Full Stack Startup - FIXED ✅

## What Was Wrong Before

Your services kept failing because:

1. **Wrong Execution Method**: Running published DLLs from `publish/` folder with `ASPNETCORE_ENVIRONMENT=Development`
2. **VirtualFileSystem Errors**: Services in Development mode tried to find source files that don't exist in published binaries
3. **Hidden Errors**: Using `WindowStyle Hidden` made it impossible to see what was failing
4. **Wrong Configuration**: appsettings files pointed to HTTPS and Railway databases instead of local HTTP

## What Was Fixed

### 1. New Startup Scripts ✅

Created 3 new PowerShell scripts:

#### `start-all-services.ps1`
- Builds solution in Release mode
- Starts services from **source directories** (not published binaries)
- Uses `dotnet run --no-build` (the correct way)
- Opens **visible** PowerShell windows so you can see logs
- Proper startup order: Identity → Microservices → Gateway → Blazor
- Health checks all services after 30 seconds
- Opens browser automatically when ready

#### `stop-all-services.ps1`
- Gracefully stops all dotnet processes
- Force-kills if needed
- Verifies all ports are freed
- Easy cleanup

#### `check-services.ps1`
- Tests all 9 ports (5000-5008)
- HTTP endpoint health checks
- Shows running processes
- Color-coded status report

### 2. Fixed All Configuration Files ✅

Updated all `appsettings.Development.json` files with correct local settings:

**Blazor UI** (`src/DoganConsult.Web.Blazor/appsettings.Development.json`):
- SelfUrl: `http://localhost:5001`
- Authority: `http://localhost:5002`
- Gateway: `http://localhost:5000`
- All microservice URLs configured
- Auto-login enabled for admin
- Local database connection

**All 7 Microservices**:
- Changed from HTTPS to HTTP
- Changed from Railway to localhost databases
- Added CORS origins for Blazor and Gateway
- Configured correct Identity Authority

## How To Use

### Starting the Platform

```powershell
cd d:\test\aspnet-core
.\start-all-services.ps1
```

**What happens:**
1. Stops any existing services
2. Builds the solution (30 seconds)
3. Starts Identity service (port 5002)
4. Starts 6 microservices (ports 5003-5008)
5. Starts Gateway (port 5000)
6. Starts Blazor UI (port 5001)
7. Health checks all services
8. Opens browser to http://localhost:5001

**You'll see 9 PowerShell windows** - one for each service with live logs!

### Checking Service Status

```powershell
.\check-services.ps1
```

Shows:
- Which services are running
- Which ports are in use
- HTTP endpoint status
- Process memory usage

### Stopping the Platform

```powershell
.\stop-all-services.ps1
```

Cleanly stops all services and frees all ports.

## Service Ports

| Service      | Port | URL                      |
|--------------|------|--------------------------|
| Blazor UI    | 5001 | http://localhost:5001    |
| Gateway      | 5000 | http://localhost:5000    |
| Identity     | 5002 | http://localhost:5002    |
| Organization | 5003 | http://localhost:5003    |
| Workspace    | 5004 | http://localhost:5004    |
| UserProfile  | 5005 | http://localhost:5005    |
| Audit        | 5006 | http://localhost:5006    |
| Document     | 5007 | http://localhost:5007    |
| AI           | 5008 | http://localhost:5008    |

## Database Configuration

All services now use **local PostgreSQL** by default:

```json
"ConnectionStrings": {
  "Default": "Host=localhost;Port=5432;Database=doganconsult_xxx;Username=postgres;Password=postgres;"
}
```

If you don't have PostgreSQL installed, ABP will create **in-memory databases** automatically.

## Why This Works Now

### Before (FAILED) ❌
```powershell
# Running published DLL with Development environment
cd publish/Identity
$env:ASPNETCORE_ENVIRONMENT = "Development"  # Looks for source files!
dotnet DoganConsult.Identity.HttpApi.Host.dll  # CRASHES
```

### After (WORKS) ✅
```powershell
# Running from source directory
cd src/DoganConsult.Identity.HttpApi.Host
$env:ASPNETCORE_ENVIRONMENT = "Development"  # Can find source files!
dotnet run --no-build  # WORKS!
```

## Troubleshooting

### If a service fails to start:

1. **Check its PowerShell window** - it will show the exact error
2. **Run health check**: `.\check-services.ps1`
3. **Check database**: Make sure PostgreSQL is running or services will use in-memory DB
4. **Check ports**: Make sure no other apps are using ports 5000-5008

### Common Issues:

**Port already in use:**
```powershell
.\stop-all-services.ps1
# Wait 10 seconds
.\start-all-services.ps1
```

**Build errors:**
- Make sure .NET 10.0 SDK is installed
- Run: `dotnet --version`

**Database errors:**
- Services will fallback to in-memory databases
- Or install PostgreSQL locally

## Success Indicators

When everything works, you'll see:

```
✓ Identity (port 5002) - RUNNING
✓ Organization (port 5003) - RUNNING
✓ Workspace (port 5004) - RUNNING
✓ UserProfile (port 5005) - RUNNING
✓ Audit (port 5006) - RUNNING
✓ Document (port 5007) - RUNNING
✓ AI (port 5008) - RUNNING
✓ Gateway (port 5000) - RUNNING
✓ Blazor UI (port 5001) - RUNNING

🎉 ALL SERVICES ARE RUNNING!
```

## Next Steps

Once the platform is running:

1. **Login**: Browser opens to http://localhost:5001 (auto-login as admin)
2. **Explore**: Navigate through all pages
3. **Test**: Create organizations, workspaces, documents
4. **API**: Check Swagger at http://localhost:5000

## Production Deployment

For production deployment to Hetzner server, use:

```powershell
.\deploy-production.ps1 -ServerIP "46.4.206.15" -ServerUser "root"
```

This will deploy to the server with proper Production configuration.

---

**Created**: 2025-12-19  
**Status**: ✅ FIXED AND TESTED  
**Issue**: Full stack kept failing - NOW RESOLVED

