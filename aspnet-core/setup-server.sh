#!/bin/bash

# Server setup script - Run this ON the server after SSH connection
# This will install Docker and prepare the environment

set -e

echo "=== DoganConsult Platform - Server Setup ==="
echo ""

# Update system
echo "Updating system packages..."
apt-get update
apt-get upgrade -y

# Install prerequisites
echo "Installing prerequisites..."
apt-get install -y \
    ca-certificates \
    curl \
    gnupg \
    lsb-release \
    git \
    ufw

# Install Docker
echo "Installing Docker..."
install -m 0755 -d /etc/apt/keyrings
curl -fsSL https://download.docker.com/linux/ubuntu/gpg -o /etc/apt/keyrings/docker.asc
chmod a+r /etc/apt/keyrings/docker.asc

echo \
  "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.asc] https://download.docker.com/linux/ubuntu \
  $(. /etc/os-release && echo "$VERSION_CODENAME") stable" | \
  tee /etc/apt/sources.list.d/docker.list > /dev/null

apt-get update
apt-get install -y docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin

# Start Docker
systemctl start docker
systemctl enable docker

# Verify Docker installation
echo ""
echo "Docker version:"
docker --version
docker compose version

# Create deployment directory
echo ""
echo "Creating deployment directory..."
mkdir -p /opt/doganconsult
cd /opt/doganconsult

# Configure firewall
echo ""
echo "Configuring firewall..."
ufw allow 22/tcp
ufw allow 80/tcp
ufw allow 443/tcp
ufw allow 5000:5010/tcp
ufw --force enable

echo ""
echo "=== Server Setup Complete ==="
echo ""
echo "Next steps:"
echo "1. Copy docker-compose.yml to /opt/doganconsult/"
echo "2. Create .env file with production settings"
echo "3. Run: docker compose up -d"
echo ""
