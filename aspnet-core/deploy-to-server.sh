#!/bin/bash
set -e

SERVER="root@46.224.64.95"
PASSWORD="As\$123456"

echo "=== DoganConsult Deployment ==="
echo "Server: 46.224.64.95"
echo ""

echo "[1/5] Checking server connection..."
sshpass -p "$PASSWORD" ssh -o StrictHostKeyChecking=no $SERVER "hostname && docker --version"

echo ""
echo "[2/5] Creating deployment directory..."
sshpass -p "$PASSWORD" ssh $SERVER "mkdir -p /opt/doganconsult/src && ls -la /opt/doganconsult"

echo ""
echo "[3/5] Transferring docker-compose.yml..."
sshpass -p "$PASSWORD" scp docker-compose.prod.yml ${SERVER}:/opt/doganconsult/docker-compose.yml

echo ""
echo "[4/5] Transferring common.props..."
sshpass -p "$PASSWORD" scp common.props ${SERVER}:/opt/doganconsult/

echo ""
echo "[5/5] Transferring source code..."
sshpass -p "$PASSWORD" scp -r src ${SERVER}:/opt/doganconsult/

echo ""
echo "Files transferred! Listing directory:"
sshpass -p "$PASSWORD" ssh $SERVER "ls -lah /opt/doganconsult"

echo ""
echo "=== Starting deployment on server ==="
sshpass -p "$PASSWORD" ssh $SERVER "cd /opt/doganconsult && docker compose build --parallel && docker compose up -d && docker compose ps"

echo ""
echo "=== Deployment Complete ==="
echo "Platform URL: http://46.224.64.95:5001"
