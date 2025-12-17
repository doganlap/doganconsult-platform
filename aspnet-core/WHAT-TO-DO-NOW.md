# What To Do Now - Deployment Checklist

## ✅ COMPLETED
1. ✓ Server configured (46.224.64.95)
2. ✓ SSH access setup (passwordless via `ssh doganconsult`)
3. ✓ Deployment scripts created
4. ✓ Docker installed on server

---

## 🔄 IN PROGRESS
- Deploying platform to server

---

## 📋 NEXT STEPS

### Step 1: Complete Deployment
**Run this command:**
```powershell
cd d:\test\aspnet-core
.\DEPLOY-NOW.ps1
```

**What it does:**
- Transfers all files to server (docker-compose.yml, source code)
- Builds Docker containers for all 9 services
- Starts the platform
- Takes ~10-20 minutes

---

### Step 2: Verify Deployment
**After deployment completes, run:**
```powershell
.\CHECK-DEPLOYMENT.ps1
```

**This checks:**
- Server connectivity
- Files transferred correctly
- Docker containers running
- Services responding on ports 5000-5008
- Recent error logs

---

### Step 3: Access Platform
**Open in browser:**
- **Main UI:** http://46.224.64.95:5001
- **API:** http://46.224.64.95:5000

---

### Step 4: If Issues Occur
**Run diagnostics:**
```powershell
.\TROUBLESHOOT.ps1
```

**Apply automatic fixes:**
```powershell
.\TROUBLESHOOT.ps1 -Fix
```

---

## 🚨 MANUAL DEPLOYMENT (If scripts fail)

### Connect to server:
```bash
ssh doganconsult
# or: ssh root@46.224.64.95
# Password: As$123456
```

### Transfer files manually:
```powershell
# From Windows, in d:\test\aspnet-core
scp docker-compose.prod.yml root@46.224.64.95:/opt/doganconsult/docker-compose.yml
scp common.props root@46.224.64.95:/opt/doganconsult/
scp -r src root@46.224.64.95:/opt/doganconsult/
```

### Build and start on server:
```bash
cd /opt/doganconsult
docker compose build --parallel
docker compose up -d
docker compose ps
```

---

## 📊 SERVICE CHECKLIST

After deployment, these should be running:

| Service | Port | Check |
|---------|------|-------|
| ☐ API Gateway | 5000 | http://46.224.64.95:5000 |
| ☐ Blazor UI | 5001 | http://46.224.64.95:5001 |
| ☐ Identity | 5002 | http://46.224.64.95:5002 |
| ☐ Organization | 5003 | http://46.224.64.95:5003 |
| ☐ Workspace | 5004 | http://46.224.64.95:5004 |
| ☐ UserProfile | 5005 | http://46.224.64.95:5005 |
| ☐ Audit | 5006 | http://46.224.64.95:5006 |
| ☐ Document | 5007 | http://46.224.64.95:5007 |
| ☐ AI Service | 5008 | http://46.224.64.95:5008 |

---

## 🔍 MONITORING

### View all logs:
```bash
ssh doganconsult
cd /opt/doganconsult
docker compose logs -f
```

### View specific service logs:
```bash
docker compose logs -f identity-service
docker compose logs -f web-ui
docker compose logs -f api-gateway
```

### Check container status:
```bash
docker compose ps
docker stats
```

---

## 🛠️ COMMON OPERATIONS

### Restart all services:
```bash
ssh doganconsult
cd /opt/doganconsult
docker compose restart
```

### Rebuild specific service:
```bash
docker compose build identity-service
docker compose up -d identity-service
```

### Stop all services:
```bash
docker compose down
```

### Clean restart:
```bash
docker compose down
docker compose build --no-cache
docker compose up -d
```

---

## 📂 FILE LOCATIONS

**On Windows:**
- Scripts: `d:\test\aspnet-core\`
- Source: `d:\test\aspnet-core\src\`
- Config: `d:\test\aspnet-core\docker-compose.prod.yml`

**On Server:**
- Deploy dir: `/opt/doganconsult/`
- Source: `/opt/doganconsult/src/`
- Config: `/opt/doganconsult/docker-compose.yml`

---

## 🎯 SUCCESS CRITERIA

Platform is successfully deployed when:

1. ✓ All 9 containers showing "Up" status
2. ✓ Blazor UI accessible at http://46.224.64.95:5001
3. ✓ No errors in `docker compose logs`
4. ✓ API Gateway responding at http://46.224.64.95:5000
5. ✓ Identity service accessible for authentication

---

## 🆘 NEED HELP?

### Quick diagnostics:
```powershell
.\quick-check.ps1
```

### Full diagnostics:
```powershell
.\TROUBLESHOOT.ps1
```

### View this checklist anytime:
```powershell
notepad WHAT-TO-DO-NOW.md
```

---

**CURRENT STATUS:** Ready to deploy  
**NEXT ACTION:** Run `.\DEPLOY-NOW.ps1`
