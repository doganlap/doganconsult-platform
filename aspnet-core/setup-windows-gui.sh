#!/bin/bash
# Setup Windows-like GUI access for Hetzner server
# Run on server: ssh root@46.224.64.95

set -e

echo "=== Installing GUI Desktop Environment ==="
apt-get update
DEBIAN_FRONTEND=noninteractive apt-get install -y \
    xfce4 xfce4-goodies xorg dbus-x11 x11-xserver-utils \
    firefox chromium-browser

echo "=== Installing Remote Desktop (XRDP) ==="
apt-get install -y xrdp
systemctl enable xrdp
systemctl start xrdp

# Configure XRDP
echo "xfce4-session" > ~/.xsession
sudo systemctl restart xrdp

echo "=== Installing Webmin (Web Management) ==="
wget -qO - http://www.webmin.com/jcameron-key.asc | apt-key add -
echo "deb http://download.webmin.com/download/repository sarge contrib" > /etc/apt/sources.list.d/webmin.list
apt-get update
apt-get install -y webmin

echo "=== Installing Cockpit (Modern Dashboard) ==="
apt-get install -y cockpit cockpit-docker
systemctl enable --now cockpit.socket

echo "=== Installing Samba (Network Drive) ==="
apt-get install -y samba samba-common-bin
mkdir -p /root/RunPod-Files
chmod 777 /root/RunPod-Files

# Configure Samba
cat > /etc/samba/smb.conf << 'EOF'
[global]
   workgroup = WORKGROUP
   server string = DoganConsult Server
   security = user
   map to guest = bad user

[RunPod-Files]
   path = /opt/doganconsult
   browseable = yes
   writable = yes
   guest ok = no
   valid users = root
   create mask = 0755
   directory mask = 0755
EOF

# Set Samba password
echo -e "samba123\nsamba123" | smbpasswd -a root

systemctl enable smbd
systemctl restart smbd

echo "=== Installing Code Server (VS Code in Browser) ==="
curl -fsSL https://code-server.dev/install.sh | sh
systemctl enable --now code-server@root

# Configure code-server
mkdir -p ~/.config/code-server
cat > ~/.config/code-server/config.yaml << 'EOF'
bind-addr: 0.0.0.0:8080
auth: password
password: admin123
cert: false
EOF

systemctl restart code-server@root

echo "=== Installing Docker Management UI ==="
docker run -d \
  --name portainer \
  --restart=always \
  -p 9000:9000 \
  -v /var/run/docker.sock:/var/run/docker.sock \
  -v portainer_data:/data \
  portainer/portainer-ce:latest

echo "=== Configuring Firewall ==="
ufw allow 22/tcp
ufw allow 3389/tcp
ufw allow 10000/tcp
ufw allow 9090/tcp
ufw allow 8080/tcp
ufw allow 9000/tcp
ufw allow 445/tcp
ufw allow 5000:5008/tcp
ufw --force enable

echo ""
echo "============================================"
echo "✅ Server Setup Complete!"
echo "============================================"
echo ""
echo "🖥️ Remote Desktop (Windows RDP):"
echo "   Address: 46.224.64.95:3389"
echo "   User: root"
echo "   Password: aKTbCKAeWapnp9xkLcjF"
echo ""
echo "📁 Network Drive (Windows File Explorer):"
echo "   \\\\46.224.64.95\\RunPod-Files"
echo "   User: root"
echo "   Password: samba123"
echo ""
echo "🌐 Web Interfaces:"
echo "   Webmin: http://46.224.64.95:10000"
echo "   Cockpit: http://46.224.64.95:9090"
echo "   VS Code: http://46.224.64.95:8080 (password: admin123)"
echo "   Portainer: http://46.224.64.95:9000"
echo ""
echo "🚀 DoganConsult Platform:"
echo "   Blazor UI: http://46.224.64.95:5001"
echo "   API Gateway: http://46.224.64.95:5000"
echo ""
