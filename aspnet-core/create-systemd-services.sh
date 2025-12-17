#!/bin/bash

# Script to create systemd service files for all services
# Run this on the Hetzner server after deployment

DEPLOY_DIR="/opt/doganconsult"
SERVICES=(
    "identity:Identity:5002"
    "organization:Organization:5003"
    "workspace:Workspace:5004"
    "userprofile:UserProfile:5005"
    "audit:Audit:5006"
    "document:Document:5007"
    "ai:AI:5008"
)

echo "Creating systemd service files..."

for service_config in "${SERVICES[@]}"; do
    IFS=':' read -r service_name service_dir port <<< "$service_config"
    
    cat > "/tmp/doganconsult-$service_name.service" << EOF
[Unit]
Description=Dogan Consult $service_dir Service
After=network.target

[Service]
Type=notify
User=doganconsult
WorkingDirectory=$DEPLOY_DIR/services/$service_dir
ExecStart=/usr/bin/dotnet $DEPLOY_DIR/services/$service_dir/DoganConsult.$service_dir.HttpApi.Host.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=doganconsult-$service_name
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://localhost:$port

[Install]
WantedBy=multi-user.target
EOF

    echo "Created service file for $service_name"
done

# Gateway service
cat > "/tmp/doganconsult-gateway.service" << EOF
[Unit]
Description=Dogan Consult API Gateway
After=network.target
After=doganconsult-identity.service

[Service]
Type=notify
User=doganconsult
WorkingDirectory=$DEPLOY_DIR/gateway
ExecStart=/usr/bin/dotnet $DEPLOY_DIR/gateway/DoganConsult.Gateway.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=doganconsult-gateway
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://localhost:5000

[Install]
WantedBy=multi-user.target
EOF

# Blazor UI service
cat > "/tmp/doganconsult-blazor.service" << EOF
[Unit]
Description=Dogan Consult Blazor UI
After=network.target
After=doganconsult-gateway.service

[Service]
Type=notify
User=doganconsult
WorkingDirectory=$DEPLOY_DIR/ui
ExecStart=/usr/bin/dotnet $DEPLOY_DIR/ui/DoganConsult.Web.Blazor.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=doganconsult-blazor
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://localhost:5001

[Install]
WantedBy=multi-user.target
EOF

echo ""
echo "Service files created in /tmp/"
echo "To install:"
echo "  sudo cp /tmp/doganconsult-*.service /etc/systemd/system/"
echo "  sudo systemctl daemon-reload"
echo "  sudo systemctl enable doganconsult-*"
echo "  sudo systemctl start doganconsult-*"
