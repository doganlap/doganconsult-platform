#!/bin/bash
sshpass -p 'aKTbCKAeWapnp9xkLcjF' ssh -o StrictHostKeyChecking=no root@46.224.64.95 'hostname && uname -a && docker --version 2>&1 || echo "Docker not installed"'
