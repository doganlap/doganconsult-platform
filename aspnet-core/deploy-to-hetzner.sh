#!/bin/bash

# Deployment script for Hetzner server (direct .NET deployment, no Docker)
# Server: 46.224.64.95
# User: doganconsult

set -e

SERVER_IP="46.224.64.95"
SERVER_USER="doganconsult"
DEPLOY_DIR="/opt/doganconsult"
SERVICES=(
    "Identity"
    "Organization"
    "Workspace"
    "UserProfile"
    "Audit"
    "Document"
    "AI"
)
GATEWAY="Gateway"
BLAZOR_UI="Web.Blazor"

echo "=== Dogan Consult Platform Deployment Script ==="
echo "Target Server: $SERVER_IP"
echo ""

# Build all projects
echo "Building solution..."
dotnet build DoganConsult.Platform.sln -c Release

# Publish each service
echo ""
echo "Publishing services..."
for service in "${SERVICES[@]}"; do
    echo "Publishing $service service..."
    dotnet publish "src/DoganConsult.$service.HttpApi.Host/DoganConsult.$service.HttpApi.Host.csproj" \
        -c Release \
        -o "publish/$service" \
        --no-build
done

# Publish Gateway
echo "Publishing API Gateway..."
dotnet publish "src/gateway/DoganConsult.Gateway/DoganConsult.Gateway.csproj" \
    -c Release \
    -o "publish/Gateway" \
    --no-build

# Publish Blazor UI
echo "Publishing Blazor UI..."
dotnet publish "src/DoganConsult.$BLAZOR_UI/DoganConsult.$BLAZOR_UI.csproj" \
    -c Release \
    -o "publish/BlazorUI" \
    --no-build

# Create deployment package
echo ""
echo "Creating deployment package..."
tar -czf deploy.tar.gz publish/

# Copy to server
echo ""
echo "Copying to server..."
scp deploy.tar.gz $SERVER_USER@$SERVER_IP:/tmp/

# Deploy on server
echo ""
echo "Deploying on server..."
ssh $SERVER_USER@$SERVER_IP << 'ENDSSH'
    set -e
    DEPLOY_DIR="/opt/doganconsult"
    
    # Create directories
    sudo mkdir -p $DEPLOY_DIR/{services,gateway,ui,logs}
    sudo mkdir -p $DEPLOY_DIR/services/{identity,organization,workspace,userprofile,audit,document,ai}
    
    # Extract deployment package
    cd /tmp
    tar -xzf deploy.tar.gz
    sudo cp -r publish/* $DEPLOY_DIR/
    
    # Set permissions
    sudo chown -R $USER:$USER $DEPLOY_DIR
    
    # Create systemd service files (will be created separately)
    echo "Deployment files copied. Next: Create systemd service files."
ENDSSH

echo ""
echo "=== Deployment Complete ==="
echo "Next steps:"
echo "1. SSH to server: ssh $SERVER_USER@$SERVER_IP"
echo "2. Create systemd service files"
echo "3. Start services: sudo systemctl start doganconsult-*"
echo "4. Enable services: sudo systemctl enable doganconsult-*"
