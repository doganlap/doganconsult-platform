#!/bin/bash

# Simple SSH connection script
# Server: 46.224.64.95
# Password: aKTbCKAeWapnp9xkLcjF

SERVER_IP="46.224.64.95"
SERVER_USER="root"
PASSWORD="aKTbCKAeWapnp9xkLcjF"

echo "Connecting to $SERVER_USER@$SERVER_IP..."
echo "Password: $PASSWORD"
echo ""
echo "Run this command to connect:"
echo "ssh $SERVER_USER@$SERVER_IP"
echo ""
echo "Or use sshpass:"
echo "sshpass -p '$PASSWORD' ssh -o StrictHostKeyChecking=no $SERVER_USER@$SERVER_IP"
