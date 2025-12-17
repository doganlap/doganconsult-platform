# Dogan Consult Operating System - Microservices Platform

## Overview

This is a microservices-based platform built with ABP Framework 10.0, .NET 10.0, PostgreSQL, and OpenIddict. The platform consists of 7 microservices, an API Gateway, and a Blazor UI.

## Architecture

- **7 Microservices**: Identity, Organization, Workspace, UserProfile, Audit, Document, AI
- **API Gateway**: YARP Reverse Proxy
- **UI**: ABP Blazor Server
- **Database**: PostgreSQL (Railway - 7 instances)
- **Cache**: Redis (Railway)
- **Authentication**: OpenIddict

## Quick Start

### Prerequisites

- .NET 10.0 SDK
- Docker & Docker Compose
- ABP CLI 10.0.1

### Running Locally

1. **Restore packages:**
   ```bash
   dotnet restore
   ```

2. **Run database migrations:**
   ```bash
   cd src/DoganConsult.Identity.DbMigrator
   dotnet run
   # Repeat for each service
   ```

3. **Start services:**
   ```bash
   # Identity Service (must start first)
   cd src/DoganConsult.Identity.HttpApi.Host
   dotnet run
   
   # Other services in separate terminals
   ```

4. **Or use Docker Compose:**
   ```bash
   docker-compose up -d
   ```

## Service Ports

- Identity: 5002
- Organization: 5003
- Workspace: 5004
- UserProfile: 5005
- Audit: 5006
- Document: 5007
- AI: 5008
- API Gateway: 5000
- Blazor UI: 5001

## API Gateway Routes

All services are accessible through the API Gateway at `http://localhost:5000`:

- `/api/identity/*` → Identity Service
- `/api/organization/*` → Organization Service
- `/api/workspace/*` → Workspace Service
- `/api/userprofile/*` → User Profile Service
- `/api/audit/*` → Audit Service
- `/api/document/*` → Document Service
- `/api/ai/*` → AI Service

## Configuration

All connection strings and settings are in `appsettings.json` files. For production, use environment variables:

- `ConnectionStrings__Default` - PostgreSQL connection
- `AuthServer__Authority` - Identity Service URL
- `Services__Identity__BaseUrl` - Identity Service base URL
- `AIService__LlmServerEndpoint` - LLM server endpoint
- `Redis__Configuration` - Redis connection string

## Deployment

See `IMPLEMENTATION_SUMMARY.md` for detailed deployment instructions.

## Documentation

- Architecture: See plan file for detailed architecture diagrams
- API Documentation: Swagger UI available at `/swagger` on each service
- Database: See plan file for database connection details
