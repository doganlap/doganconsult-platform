# Deploy DoganConsult Platform - Quick Guide

## Automated (Run in PowerShell)

```powershell
cd d:\test\aspnet-core
.\quick-deploy.ps1
```

This will:
1. Download WinSCP
2. Transfer all files to server
3. Build and start Docker containers
4. Create desktop shortcut
5. Open platform in browser

---

## Manual Method

### Step 1: Transfer Files

**Using WinSCP:**
1. Download: https://winscp.net/download/WinSCP-6.3.5-Setup.exe
2. Connect:
   - Host: `46.224.64.95`
   - User: `root`
   - Password: `aKTbCKAeWapnp9xkLcjF`
3. Navigate to `/opt/doganconsult`
4. Upload:
   - `docker-compose.prod.yml` → rename to `docker-compose.yml`
   - `common.props`
   - `src` folder (entire directory)

**Using PowerShell SCP:**
```powershell
cd d:\test\aspnet-core
scp docker-compose.prod.yml root@46.224.64.95:/opt/doganconsult/docker-compose.yml
scp common.props root@46.224.64.95:/opt/doganconsult/
scp -r src root@46.224.64.95:/opt/doganconsult/
```

### Step 2: Build & Start on Server

Connect to server:
```powershell
ssh root@46.224.64.95
# Password: aKTbCKAeWapnp9xkLcjF
```

Run on server:
```bash
cd /opt/doganconsult
docker compose build --parallel
docker compose up -d
docker compose ps
```

### Step 3: Access Platform

Open browser: **http://46.224.64.95:5001**

---

## Install GUI Tools (Optional)

On server, run:
```bash
# Install Remote Desktop
apt-get update
apt-get install -y xrdp xfce4
systemctl enable xrdp
systemctl start xrdp
ufw allow 3389/tcp

# Install Webmin
curl -fsSL https://download.webmin.com/jcameron-key.asc | gpg --dearmor -o /usr/share/keyrings/webmin.gpg
echo "deb [signed-by=/usr/share/keyrings/webmin.gpg] https://download.webmin.com/download/repository sarge contrib" > /etc/apt/sources.list.d/webmin.list
apt-get update && apt-get install -y webmin
ufw allow 10000/tcp

# Install Portainer (Docker UI)
docker run -d --name portainer --restart=always \
  -p 9000:9000 -v /var/run/docker.sock:/var/run/docker.sock \
  -v portainer_data:/data portainer/portainer-ce
ufw allow 9000/tcp
```

**Access:**
- **Remote Desktop:** Use Windows RDP → `46.224.64.95:3389`
- **Webmin:** http://46.224.64.95:10000
- **Portainer:** http://46.224.64.95:9000

---

## Service URLs

| Service | URL |
|---------|-----|
| **Platform UI** | http://46.224.64.95:5001 |
| API Gateway | http://46.224.64.95:5000 |
| Identity | http://46.224.64.95:5002 |
| Organization | http://46.224.64.95:5003 |
| Workspace | http://46.224.64.95:5004 |
| UserProfile | http://46.224.64.95:5005 |
| Audit | http://46.224.64.95:5006 |
| Document | http://46.224.64.95:5007 |
| AI Service | http://46.224.64.95:5008 |

---

## Troubleshooting

**Check logs:**
```bash
docker compose logs -f
docker compose logs identity-service
```

**Restart services:**
```bash
docker compose restart
```

**Rebuild:**
```bash
docker compose down
docker compose build --no-cache
docker compose up -d
```

**Check firewall:**
```bash
ufw status
ufw allow 5000:5008/tcp
```
