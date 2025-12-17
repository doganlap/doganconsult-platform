#!/bin/bash

# Setup script to run on Hetzner server
# This script sets up systemd services for all microservices

set -e

DEPLOY_PATH="/opt/doganconsult"
SERVICES_PATH="$DEPLOY_PATH/publish"
SYSTEMD_PATH="/etc/systemd/system"

echo "=== Setting up Dogan Consult Services ==="

# Create application user if it doesn't exist
if ! id "doganconsult" &>/dev/null; then
    useradd -r -s /bin/bash -d $DEPLOY_PATH doganconsult
    echo "Created user: doganconsult"
fi

# Create directories
mkdir -p $DEPLOY_PATH/{logs,ssl,config}
chown -R doganconsult:doganconsult $DEPLOY_PATH

# Install .NET 10.0 Runtime if not installed
if ! command -v dotnet &> /dev/null; then
    echo "Installing .NET 10.0 Runtime..."
    wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
    chmod +x dotnet-install.sh
    ./dotnet-install.sh --channel 10.0 --runtime aspnetcore
    export PATH="$PATH:$HOME/.dotnet"
    echo 'export PATH="$PATH:$HOME/.dotnet"' >> ~/.bashrc
fi

# Create systemd service files
create_service() {
    local service_name=$1
    local service_path=$2
    local port=$3
    local working_dir="$SERVICES_PATH/$service_name"
    
    cat > "$SYSTEMD_PATH/doganconsult-$service_name.service" <<EOF
[Unit]
Description=Dogan Consult $service_name Service
After=network.target

[Service]
Type=notify
User=doganconsult
Group=doganconsult
WorkingDirectory=$working_dir
ExecStart=/root/.dotnet/dotnet $working_dir/DoganConsult.$service_name.HttpApi.Host.dll
Restart=always
RestartSec=10
StandardOutput=journal
StandardError=journal
SyslogIdentifier=doganconsult-$service_name
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://0.0.0.0:$port

[Install]
WantedBy=multi-user.target
EOF

    echo "Created service: doganconsult-$service_name.service"
}

# Create services
create_service "Identity" "Identity" 5002
create_service "Organization" "Organization" 5003
create_service "Workspace" "Workspace" 5004
create_service "UserProfile" "UserProfile" 5005
create_service "Audit" "Audit" 5006
create_service "Document" "Document" 5007
create_service "AI" "AI" 5008

# Gateway service (different structure)
cat > "$SYSTEMD_PATH/doganconsult-gateway.service" <<EOF
[Unit]
Description=Dogan Consult API Gateway
After=network.target

[Service]
Type=notify
User=doganconsult
Group=doganconsult
WorkingDirectory=$SERVICES_PATH/Gateway
ExecStart=/root/.dotnet/dotnet $SERVICES_PATH/Gateway/DoganConsult.Gateway.dll
Restart=always
RestartSec=10
StandardOutput=journal
StandardError=journal
SyslogIdentifier=doganconsult-gateway
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://0.0.0.0:5000

[Install]
WantedBy=multi-user.target
EOF

# Blazor UI service
cat > "$SYSTEMD_PATH/doganconsult-blazor.service" <<EOF
[Unit]
Description=Dogan Consult Blazor UI
After=network.target

[Service]
Type=notify
User=doganconsult
Group=doganconsult
WorkingDirectory=$SERVICES_PATH/Blazor
ExecStart=/root/.dotnet/dotnet $SERVICES_PATH/Blazor/DoganConsult.Web.Blazor.dll
Restart=always
RestartSec=10
StandardOutput=journal
StandardError=journal
SyslogIdentifier=doganconsult-blazor
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://0.0.0.0:5001

[Install]
WantedBy=multi-user.target
EOF

# Reload systemd
systemctl daemon-reload

echo ""
echo "=== Services Created ==="
echo "To start all services:"
echo "  systemctl start doganconsult-*"
echo ""
echo "To enable services on boot:"
echo "  systemctl enable doganconsult-*"
echo ""
echo "To check status:"
echo "  systemctl status doganconsult-identity"
echo ""
