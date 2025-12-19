# Quick Start Guide - DoganConsult Platform

## 🚀 Three Easy Deployment Options

### Option 1: Local Deployment (Fastest for Testing)

Run all services locally on your machine:

```powershell
# Start all services
.\start-local.ps1

# Stop all services
.\stop-local.ps1
```

**Access URLs:**
- Blazor UI: http://localhost:5001
- API Gateway: http://localhost:5000
- Identity: http://localhost:5002

---

### Option 2: Docker Deployment (Recommended for Production)

Deploy using Docker containers:

```powershell
# Build and start all services
docker-compose -f docker-compose.local.yml up -d

# View logs
docker-compose -f docker-compose.local.yml logs -f

# Stop services
docker-compose -f docker-compose.local.yml down
```

**Access URLs:**
- Blazor UI: http://localhost:5001
- API Gateway: http://localhost:5000

---

### Option 3: Easy Setup Script (All-in-One)

Use the automated setup script:

```powershell
# Local deployment
.\easy-setup.ps1 -DeploymentType Local

# Docker deployment
.\easy-setup.ps1 -DeploymentType Docker

# Server deployment
.\easy-setup.ps1 -DeploymentType Server -ServerIP "46.4.206.15"
```

---

## 📋 Prerequisites

### For Local Deployment:
- .NET 10.0 SDK
- Windows PowerShell 7+ or PowerShell Core

### For Docker Deployment:
- Docker Desktop
- .NET 10.0 SDK (for building)

### For Server Deployment:
- SSH access to server
- .NET 10.0 SDK (for building)
- Server with Ubuntu/Linux

---

## 🔧 Service Ports

| Service | Port | URL |
|---------|------|-----|
| Blazor UI | 5001 | http://localhost:5001 |
| API Gateway | 5000 | http://localhost:5000 |
| Identity | 5002 | http://localhost:5002 |
| Organization | 5003 | http://localhost:5003 |
| Workspace | 5004 | http://localhost:5004 |
| UserProfile | 5005 | http://localhost:5005 |
| Audit | 5006 | http://localhost:5006 |
| Document | 5007 | http://localhost:5007 |
| AI | 5008 | http://localhost:5008 |

---

## 🐛 Troubleshooting

### Port Already in Use
If a port is already in use:
```powershell
# Find process using port
netstat -ano | findstr :5001

# Kill process (replace PID with actual process ID)
taskkill /PID <PID> /F
```

### Services Won't Start
1. Check if .NET SDK is installed: `dotnet --version`
2. Rebuild solution: `dotnet build DoganConsult.Platform.sln -c Release`
3. Check logs in service directories

### Docker Issues
1. Ensure Docker Desktop is running
2. Check Docker logs: `docker-compose -f docker-compose.local.yml logs`
3. Rebuild images: `docker-compose -f docker-compose.local.yml build --no-cache`

---

## 📝 Next Steps

1. **Access the UI**: Open http://localhost:5001 in your browser
2. **Check Health**: Visit http://localhost:5000/health (Gateway)
3. **View Logs**: Check console output or Docker logs
4. **Configure**: Edit `appsettings.json` files as needed

---

## 🔗 More Information

- Full deployment guide: `DEPLOYMENT_STATUS.md`
- Production deployment: `deploy-production.ps1`
- Server setup: `deployment/setup-services.sh`

---

**Need Help?** Check the logs or see `DEPLOYMENT_STATUS.md` for detailed information.

