#!/bin/bash

# Script to run database migrations for all services

set -e

echo "=== Running Database Migrations ==="
echo ""

SERVICES=(
    "Identity"
    "Organization"
    "Workspace"
    "UserProfile"
    "Audit"
    "Document"
    "AI"
)

for service in "${SERVICES[@]}"; do
    echo "Running migrations for $service service..."
    cd "src/DoganConsult.$service.DbMigrator"
    
    if dotnet run; then
        echo "✓ $service migrations completed"
    else
        echo "✗ $service migrations failed"
        exit 1
    fi
    
    cd ../../..
    echo ""
done

echo "=== All Migrations Complete ==="
