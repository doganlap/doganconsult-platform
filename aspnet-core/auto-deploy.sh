#!/bin/bash

# Automated deployment script using sshpass
SERVER_IP="46.224.64.95"
SERVER_USER="root"
SERVER_PASS="As\$123456"

echo "=== DoganConsult Platform - Auto Deploy ==="
echo "Target: $SERVER_USER@$SERVER_IP"
echo ""

# Function to run SSH commands
run_ssh() {
    sshpass -p "$SERVER_PASS" ssh -o StrictHostKeyChecking=no "$SERVER_USER@$SERVER_IP" "$1"
}

# Function to copy files
copy_files() {
    sshpass -p "$SERVER_PASS" scp -o StrictHostKeyChecking=no -r "$1" "$SERVER_USER@$SERVER_IP:$2"
}

# Step 1: Test connection
echo "Step 1: Testing connection..."
run_ssh "echo 'Connected successfully' && hostname && uname -a"

if [ $? -ne 0 ]; then
    echo "ERROR: Cannot connect to server"
    exit 1
fi

# Step 2: Install Docker
echo ""
echo "Step 2: Installing Docker..."
run_ssh "
    apt-get update && \
    apt-get install -y ca-certificates curl gnupg && \
    install -m 0755 -d /etc/apt/keyrings && \
    curl -fsSL https://download.docker.com/linux/ubuntu/gpg -o /etc/apt/keyrings/docker.asc && \
    chmod a+r /etc/apt/keyrings/docker.asc && \
    echo \"deb [arch=\$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.asc] https://download.docker.com/linux/ubuntu \$(. /etc/os-release && echo \\\$VERSION_CODENAME) stable\" | tee /etc/apt/sources.list.d/docker.list > /dev/null && \
    apt-get update && \
    apt-get install -y docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin && \
    systemctl start docker && \
    systemctl enable docker && \
    docker --version
"

# Step 3: Create directories
echo ""
echo "Step 3: Creating directories..."
run_ssh "mkdir -p /opt/doganconsult && cd /opt/doganconsult && pwd"

# Step 4: Copy files
echo ""
echo "Step 4: Copying files to server..."
copy_files "docker-compose.prod.yml" "/opt/doganconsult/docker-compose.yml"
copy_files "src" "/opt/doganconsult/"
copy_files "common.props" "/opt/doganconsult/"

# Step 5: Build and start services
echo ""
echo "Step 5: Building and starting services..."
run_ssh "
    cd /opt/doganconsult && \
    docker compose build && \
    docker compose up -d
"

# Step 6: Check status
echo ""
echo "Step 6: Checking service status..."
run_ssh "cd /opt/doganconsult && docker compose ps"

echo ""
echo "=== Deployment Complete ==="
echo "Access your application at:"
echo "  - Blazor UI: http://46.224.64.95:5001"
echo "  - API Gateway: http://46.224.64.95:5000"
echo ""
echo "To view logs:"
echo "  ssh $SERVER_USER@$SERVER_IP 'cd /opt/doganconsult && docker compose logs -f'"
