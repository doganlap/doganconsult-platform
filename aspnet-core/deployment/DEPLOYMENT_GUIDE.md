# Deployment Guide - Hetzner Server (No Docker)

This guide covers deploying the Dogan Consult microservices platform directly to a Hetzner server without Docker.

## Prerequisites

- Hetzner server with Ubuntu (46.224.64.95)
- SSH access to the server
- .NET 10.0 SDK installed locally
- All database migrations completed

## Step 1: Run Database Migrations Locally

```powershell
# On Windows
.\run-migrations.ps1

# Or on Linux/Mac
bash run-migrations.sh
```

This will run migrations for all 7 services against the Railway PostgreSQL instances.

## Step 2: Build and Publish Services

```powershell
# Build solution
dotnet build DoganConsult.Platform.sln -c Release

# Publish all services
.\deploy-to-hetzner.ps1 -ServerIP 46.224.64.95 -ServerUser root
```

Or manually publish each service:

```powershell
# Identity Service
dotnet publish src/DoganConsult.Identity.HttpApi.Host -c Release -o publish/Identity

# Organization Service
dotnet publish src/DoganConsult.Organization.HttpApi.Host -c Release -o publish/Organization

# Workspace Service
dotnet publish src/DoganConsult.Workspace.HttpApi.Host -c Release -o publish/Workspace

# UserProfile Service
dotnet publish src/DoganConsult.UserProfile.HttpApi.Host -c Release -o publish/UserProfile

# Audit Service
dotnet publish src/DoganConsult.Audit.HttpApi.Host -c Release -o publish/Audit

# Document Service
dotnet publish src/DoganConsult.Document.HttpApi.Host -c Release -o publish/Document

# AI Service
dotnet publish src/DoganConsult.AI.HttpApi.Host -c Release -o publish/AI

# API Gateway
dotnet publish src/gateway/DoganConsult.Gateway -c Release -o publish/Gateway

# Blazor UI
dotnet publish src/DoganConsult.Web.Blazor -c Release -o publish/Blazor
```

## Step 3: Copy Files to Server

```bash
# Copy publish folder to server
scp -r publish root@46.224.64.95:/opt/doganconsult/

# Copy deployment scripts
scp -r deployment/* root@46.224.64.95:/opt/doganconsult/
```

## Step 4: Setup Server

SSH to the server:

```bash
ssh root@46.224.64.95
```

Run the setup script:

```bash
cd /opt/doganconsult
chmod +x setup-services.sh
./setup-services.sh
```

## Step 5: Configure Nginx

```bash
# Copy nginx config
cp /opt/doganconsult/nginx-config.conf /etc/nginx/sites-available/doganconsult

# Create symlink
ln -s /etc/nginx/sites-available/doganconsult /etc/nginx/sites-enabled/

# Remove default site
rm /etc/nginx/sites-enabled/default

# Test nginx config
nginx -t

# Reload nginx
systemctl reload nginx
```

## Step 6: Setup SSL (Let's Encrypt)

```bash
# Install certbot if not installed
apt install certbot python3-certbot-nginx -y

# Get SSL certificate (replace with your domain)
certbot --nginx -d your-domain.com -d www.your-domain.com
```

## Step 7: Start Services

```bash
# Start all services
systemctl start doganconsult-identity
systemctl start doganconsult-organization
systemctl start doganconsult-workspace
systemctl start doganconsult-userprofile
systemctl start doganconsult-audit
systemctl start doganconsult-document
systemctl start doganconsult-ai
systemctl start doganconsult-gateway
systemctl start doganconsult-blazor

# Enable services to start on boot
systemctl enable doganconsult-*

# Check status
systemctl status doganconsult-identity
```

## Step 8: Verify Deployment

```bash
# Check all services are running
systemctl list-units | grep doganconsult

# Check logs
journalctl -u doganconsult-identity -f
journalctl -u doganconsult-gateway -f

# Test API Gateway
curl http://localhost:5000/api/organization/organizations

# Test Identity Service
curl http://localhost:5002/connect/token
```

## Service Management

### Start/Stop Services

```bash
# Start a service
systemctl start doganconsult-identity

# Stop a service
systemctl stop doganconsult-identity

# Restart a service
systemctl restart doganconsult-identity

# Check status
systemctl status doganconsult-identity
```

### View Logs

```bash
# View logs for a service
journalctl -u doganconsult-identity -f

# View last 100 lines
journalctl -u doganconsult-identity -n 100

# View logs since today
journalctl -u doganconsult-identity --since today
```

### Update a Service

```bash
# Stop the service
systemctl stop doganconsult-identity

# Copy new files
# (from your local machine)
scp -r publish/Identity/* root@46.224.64.95:/opt/doganconsult/publish/Identity/

# Start the service
systemctl start doganconsult-identity
```

## Firewall Configuration

```bash
# Allow SSH
ufw allow 22/tcp

# Allow HTTP/HTTPS
ufw allow 80/tcp
ufw allow 443/tcp

# Enable firewall
ufw enable

# Check status
ufw status
```

## Monitoring

### Health Checks

Each service should have a health check endpoint. Test them:

```bash
curl http://localhost:5002/health
curl http://localhost:5003/health
# etc.
```

### Resource Monitoring

```bash
# Check CPU and memory usage
htop

# Check disk usage
df -h

# Check service resource usage
systemctl status doganconsult-identity
```

## Troubleshooting

### Service Won't Start

1. Check logs: `journalctl -u doganconsult-identity -n 50`
2. Check if port is in use: `netstat -tulpn | grep 5002`
3. Verify .NET runtime: `dotnet --version`
4. Check file permissions: `ls -la /opt/doganconsult/publish/Identity/`

### Database Connection Issues

1. Verify connection strings in appsettings.json
2. Test database connection: `psql -h nozomi.proxy.rlwy.net -p 35537 -U postgres -d railway`
3. Check firewall rules on Railway

### High Memory Usage

1. Check which service is using memory: `systemctl status doganconsult-*`
2. Consider restarting services during low-traffic periods
3. Monitor with: `journalctl -u doganconsult-* --since 1 hour ago`

## Backup Strategy

### Database Backups

Railway handles PostgreSQL backups automatically, but you can also:

1. Export data: `pg_dump -h nozomi.proxy.rlwy.net -p 35537 -U postgres railway > backup.sql`
2. Schedule regular backups using cron

### Application Backups

```bash
# Backup published applications
tar -czf doganconsult-backup-$(date +%Y%m%d).tar.gz /opt/doganconsult/publish

# Backup configuration
tar -czf doganconsult-config-$(date +%Y%m%d).tar.gz /opt/doganconsult/config /etc/systemd/system/doganconsult-*.service
```

## Next Steps

1. Set up monitoring (Prometheus, Grafana)
2. Configure log aggregation (ELK stack or similar)
3. Set up automated backups
4. Configure CI/CD pipeline for automated deployments
5. Set up alerting for service failures
