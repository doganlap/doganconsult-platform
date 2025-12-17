# Deployment Guide - Direct .NET Deployment (No Docker)

This guide covers deploying the Dogan Consult platform directly to the Hetzner server without Docker.

## Prerequisites

- .NET 10.0 Runtime installed on Hetzner server
- SSH access to server (46.224.64.95)
- All database migrations completed
- Railway PostgreSQL and Redis connections configured

## Server Setup

### 1. Install .NET 10.0 Runtime on Server

```bash
ssh root@46.224.64.95

# Install .NET 10.0 Runtime
wget https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --channel 10.0 --runtime aspnetcore

# Add to PATH
echo 'export DOTNET_ROOT=$HOME/.dotnet' >> ~/.bashrc
echo 'export PATH=$PATH:$HOME/.dotnet' >> ~/.bashrc
source ~/.bashrc

# Verify installation
dotnet --version
```

### 2. Create Application User and Directories

```bash
# Create user
useradd -m -s /bin/bash doganconsult

# Create directories
mkdir -p /opt/doganconsult/{services,gateway,ui,logs}
mkdir -p /opt/doganconsult/services/{identity,organization,workspace,userprofile,audit,document,ai}

# Set permissions
chown -R doganconsult:doganconsult /opt/doganconsult
```

### 3. Install Nginx (Reverse Proxy)

```bash
apt update
apt install nginx -y

# Configure Nginx (see nginx-config.conf)
# Enable and start
systemctl enable nginx
systemctl start nginx
```

## Local Build and Deployment

### Step 1: Run Database Migrations

```bash
cd aspnet-core
chmod +x run-migrations.sh
./run-migrations.sh
```

Or manually:
```bash
# For each service
cd src/DoganConsult.Identity.DbMigrator
dotnet run
# Repeat for Organization, Workspace, UserProfile, Audit, Document, AI
```

### Step 2: Build and Publish

```bash
# Build solution
dotnet build DoganConsult.Platform.sln -c Release

# Publish each service
dotnet publish src/DoganConsult.Identity.HttpApi.Host/DoganConsult.Identity.HttpApi.Host.csproj \
    -c Release -o publish/Identity

dotnet publish src/DoganConsult.Organization.HttpApi.Host/DoganConsult.Organization.HttpApi.Host.csproj \
    -c Release -o publish/Organization

# ... repeat for all services

# Publish Gateway
dotnet publish src/gateway/DoganConsult.Gateway/DoganConsult.Gateway.csproj \
    -c Release -o publish/Gateway

# Publish Blazor UI
dotnet publish src/DoganConsult.Web.Blazor/DoganConsult.Web.Blazor.csproj \
    -c Release -o publish/BlazorUI
```

### Step 3: Deploy to Server

```bash
# Use deployment script
chmod +x deploy-to-hetzner.sh
./deploy-to-hetzner.sh

# Or manually:
tar -czf deploy.tar.gz publish/
scp deploy.tar.gz doganconsult@46.224.64.95:/tmp/

# On server:
ssh doganconsult@46.224.64.95
cd /tmp
tar -xzf deploy.tar.gz
sudo cp -r publish/* /opt/doganconsult/
```

### Step 4: Create Systemd Services

On the server:

```bash
# Copy service creation script
scp create-systemd-services.sh doganconsult@46.224.64.95:/tmp/

# On server
chmod +x /tmp/create-systemd-services.sh
/tmp/create-systemd-services.sh

# Install services
sudo cp /tmp/doganconsult-*.service /etc/systemd/system/
sudo systemctl daemon-reload
sudo systemctl enable doganconsult-*
sudo systemctl start doganconsult-*
```

### Step 5: Configure Nginx

Create `/etc/nginx/sites-available/doganconsult`:

```nginx
upstream api_gateway {
    server localhost:5000;
}

upstream blazor_ui {
    server localhost:5001;
}

server {
    listen 80;
    server_name your-domain.com www.your-domain.com;
    
    # API Gateway
    location /api/ {
        proxy_pass http://api_gateway;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
    
    # Blazor UI
    location / {
        proxy_pass http://blazor_ui;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection "upgrade";
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

Enable site:
```bash
sudo ln -s /etc/nginx/sites-available/doganconsult /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl reload nginx
```

## Service Management

### Check Service Status

```bash
sudo systemctl status doganconsult-identity
sudo systemctl status doganconsult-organization
# ... etc
```

### View Logs

```bash
sudo journalctl -u doganconsult-identity -f
sudo journalctl -u doganconsult-organization -f
# ... etc
```

### Restart Services

```bash
sudo systemctl restart doganconsult-identity
# Or restart all
sudo systemctl restart doganconsult-*
```

### Stop Services

```bash
sudo systemctl stop doganconsult-*
```

## Environment Variables

Create `/opt/doganconsult/.env` or set in systemd service files:

```bash
# PostgreSQL (Railway)
ConnectionStrings__Default=Host=nozomi.proxy.rlwy.net;Port=35537;Database=railway;Username=postgres;Password=...;SSL Mode=Require;

# Identity Service URL
AuthServer__Authority=https://your-domain.com
Services__Identity__BaseUrl=https://your-domain.com

# AI Service
AIService__LlmServerEndpoint=https://hertze.your-domain.com/api/v1
AIService__ApiKey=your-api-key

# Redis
Redis__Configuration=interchange.proxy.rlwy.net:26424,password=...,ssl=false
```

## Health Checks

Test services locally on server:

```bash
# Identity Service
curl http://localhost:5002/health

# Organization Service
curl http://localhost:5003/health

# API Gateway
curl http://localhost:5000/api/organization/organizations
```

## Troubleshooting

1. **Service won't start**: Check logs with `journalctl -u doganconsult-<service> -n 50`
2. **Database connection failed**: Verify connection strings and Railway credentials
3. **Port already in use**: Check with `netstat -tulpn | grep <port>`
4. **Permission denied**: Ensure `doganconsult` user owns `/opt/doganconsult`

## Update Deployment

To update a service:

```bash
# 1. Build and publish locally
dotnet publish src/DoganConsult.<Service>.HttpApi.Host/... -c Release -o publish/<Service>

# 2. Copy to server
scp -r publish/<Service>/* doganconsult@46.224.64.95:/opt/doganconsult/services/<Service>/

# 3. Restart service
ssh doganconsult@46.224.64.95
sudo systemctl restart doganconsult-<service>
```
