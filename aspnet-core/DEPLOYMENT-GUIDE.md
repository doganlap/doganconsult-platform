# DoganConsult Platform - Production Deployment Guide

## Server Details
- **IP**: 46.224.64.95
- **User**: root
- **Password**: aKTbCKAeWapnp9xkLcjF
- **OS**: Ubuntu
- **Resources**: 16 vCPU, 32GB RAM, 640GB Disk

## Quick Start Deployment

### Step 1: Connect to Server
```bash
ssh root@46.224.64.95
# Password: aKTbCKAeWapnp9xkLcjF
```

### Step 2: Download and Run Setup Script
On the server, run:
```bash
curl -o setup-server.sh https://raw.githubusercontent.com/yourusername/yourrepo/main/setup-server.sh
chmod +x setup-server.sh
./setup-server.sh
```

Or copy the `setup-server.sh` file manually and run it.

### Step 3: Transfer Project Files
From your local machine:
```bash
# Option 1: Using SCP (from aspnet-core directory)
scp -r . root@46.224.64.95:/opt/doganconsult/

# Option 2: Using Git (on server)
cd /opt/doganconsult
git clone https://github.com/yourusername/DoganConsult.git .
```

### Step 4: Copy Production Docker Compose
```bash
cd /opt/doganconsult
cp docker-compose.prod.yml docker-compose.yml
```

### Step 5: Build and Start Services
```bash
cd /opt/doganconsult
docker compose build
docker compose up -d
```

### Step 6: Run Database Migrations
```bash
# Run migrations for each service
docker compose exec identity-service dotnet DoganConsult.DbMigrator.dll
```

### Step 7: Check Services Status
```bash
docker compose ps
docker compose logs -f
```

## Service URLs
After deployment, services will be available at:
- **Blazor UI**: http://46.224.64.95:5001
- **API Gateway**: http://46.224.64.95:5000
- **Identity Service**: http://46.224.64.95:5002
- **Organization Service**: http://46.224.64.95:5003
- **Workspace Service**: http://46.224.64.95:5004
- **UserProfile Service**: http://46.224.64.95:5005
- **Audit Service**: http://46.224.64.95:5006
- **Document Service**: http://46.224.64.95:5007
- **AI Service**: http://46.224.64.95:5008

## Firewall Configuration
The setup script configures UFW to allow:
- Port 22 (SSH)
- Port 80 (HTTP)
- Port 443 (HTTPS)
- Ports 5000-5010 (Application services)

## Troubleshooting

### Check Service Logs
```bash
# All services
docker compose logs -f

# Specific service
docker compose logs -f identity-service
```

### Restart Services
```bash
# Restart all
docker compose restart

# Restart specific service
docker compose restart identity-service
```

### Stop Services
```bash
docker compose down
```

### Rebuild After Code Changes
```bash
docker compose down
docker compose build --no-cache
docker compose up -d
```

## Database Connections
The platform uses Railway PostgreSQL instances:
- Identity: nozomi.proxy.rlwy.net:35537
- Organization: metro.proxy.rlwy.net:47319
- Workspace: switchyard.proxy.rlwy.net:37561
- UserProfile: hopper.proxy.rlwy.net:47669
- Audit: crossover.proxy.rlwy.net:17109
- Document: yamanote.proxy.rlwy.net:35357
- AI: ballast.proxy.rlwy.net:53629

## Redis Cache
- Host: interchange.proxy.rlwy.net:26424
- Used by: AI Service

## Security Recommendations
1. Change the root password after first login
2. Create a non-root user for deployment
3. Set up SSL certificates for HTTPS
4. Configure reverse proxy (Nginx/Caddy) for better security
5. Enable automatic security updates
6. Set up monitoring and alerting

## Next Steps for Production
1. **Domain Setup**: Point your domain to 46.224.64.95
2. **SSL/TLS**: Set up Let's Encrypt certificates
3. **Reverse Proxy**: Configure Nginx or Caddy
4. **Monitoring**: Set up health checks and logging
5. **Backups**: Configure automated database backups
6. **CI/CD**: Set up GitHub Actions for automated deployments
