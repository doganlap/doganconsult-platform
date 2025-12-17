# Connect to DoganConsult Server - Quick Access

## Server Details
- **IP:** 46.224.64.95
- **User:** root
- **Password:** As$123456

---

## ⚡ ONE-CLICK CONNECTION

### Method 1: Double-click BAT file (Easiest)
📁 File: `d:\test\aspnet-core\connect-server.bat`

Just double-click this file to connect via SSH!

### Method 2: PowerShell Command
```powershell
ssh root@46.224.64.95
```
Password: `As$123456`

### Method 3: PuTTY Saved Session
1. Open PuTTY
2. Load session: **DoganConsult-Server**
3. Click Open
4. Enter password: `As$123456`

---

## 🔑 SETUP PASSWORD-LESS LOGIN (Optional)

Run this once to never type password again:
```powershell
cd d:\test\aspnet-core
.\setup-ssh-key.ps1
```

After this, SSH will connect automatically without asking for password!

---

## 🖥️ CREATE DESKTOP SHORTCUTS

Run this to create shortcuts on your desktop:
```powershell
cd d:\test\aspnet-core
.\create-desktop-shortcuts.ps1
```

This creates:
- ✅ **SSH-DoganConsult-Server.bat** - One-click SSH
- ✅ **DoganConsult-Platform.url** - Open platform in browser
- ✅ **DoganConsult-RDP.rdp** - Remote Desktop connection
- ✅ **DoganConsult-Webmin.url** - Server management
- ✅ **DoganConsult-Portainer.url** - Docker management

---

## 🚀 QUICK DEPLOYMENT

To deploy the platform:
```powershell
cd d:\test\aspnet-core
.\DEPLOY.bat
```

Or use the PowerShell script:
```powershell
cd d:\test\aspnet-core
.\quick-deploy.ps1
```

---

## 📱 ACCESS SERVICES

| Service | URL |
|---------|-----|
| Platform UI | http://46.224.64.95:5001 |
| API Gateway | http://46.224.64.95:5000 |
| Webmin | http://46.224.64.95:10000 |
| Cockpit | http://46.224.64.95:9090 |
| Portainer | http://46.224.64.95:9000 |
| Remote Desktop | 46.224.64.95:3389 |

---

## 📂 FILES SUMMARY

All these files are in `d:\test\aspnet-core\`:

**Connection:**
- `connect-server.bat` - SSH connection
- `connect-server.sh` - Linux/WSL SSH
- `setup-ssh-key.ps1` - Setup passwordless login
- `setup-putty-session.ps1` - Configure PuTTY
- `create-desktop-shortcuts.ps1` - Desktop shortcuts

**Deployment:**
- `DEPLOY.bat` - Simple deployment
- `quick-deploy.ps1` - PowerShell deployment
- `deploy-production.ps1` - Full production deploy
- `auto-deploy.sh` - Linux automated deploy

**Server Setup:**
- `setup-windows-gui.sh` - Install GUI tools on server
- `docker-compose.prod.yml` - Production Docker config
