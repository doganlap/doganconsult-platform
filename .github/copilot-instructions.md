# DoganConsult Platform - AI Agent Instructions

## Architecture Overview

This is a **microservices platform** built with **ABP Framework 10.0** and **.NET 10.0**, featuring 7 specialized services orchestrated through a YARP-based API Gateway. Each service follows ABP's modular architecture with dedicated PostgreSQL databases.

### Core Services
- **Identity Service** (OpenIddict auth) - `src/DoganConsult.Identity.*`
- **Organization Service** (client management) - `src/DoganConsult.Organization.*`
- **AI Service** (multi-model LLM integration) - `src/DoganConsult.AI.*`
- **Web UI** (Blazor Server) - `src/DoganConsult.Web.*`
- **Workspace, UserProfile, Audit, Document** services (domain-specific)
- **API Gateway** - `src/gateway/DoganConsult.Gateway/`

Each service follows the **ABP layered architecture**:
```
*.Domain (entities, domain services)
*.Domain.Shared (DTOs, enums, constants)
*.Application (app services, business logic)
*.Application.Contracts (interfaces, DTOs)
*.EntityFrameworkCore (data access, repositories)
*.HttpApi (controllers)
*.HttpApi.Host (startup, hosting)
*.DbMigrator (database seeding)
```

## Development Workflow

### Database Migrations
- **Create migrations**: `./create-migrations.ps1` (creates for all services)
- **Run migrations**: `./run-migrations.ps1` (applies to all databases)
- **Service order matters**: Identity service must be migrated first

### Service Startup
- **Always start Identity service first** (port 5002) - other services depend on it
- Services use **Railway PostgreSQL** instances (7 separate databases)
- **Redis caching** configured for performance
- **Docker Compose** available: `docker-compose up -d`

### Build & Test
- **Master solution**: `DoganConsult.Platform.sln` (includes all projects)
- **Individual solutions** available per service (e.g., `DoganConsult.AI.slnx`)
- Common properties defined in `common.props`

## Key Patterns & Conventions

### ABP Framework Standards
- Use `ApplicationService` base for business logic
- Implement `IApplicationService` for public APIs
- DTOs must be in `*.Application.Contracts` projects
- Controllers should be thin wrappers around App Services
- Example: [DoganConsult.Organization.HttpApi/Controllers/Organizations/OrganizationController.cs](aspnet-core/src/DoganConsult.Organization.HttpApi/Controllers/Organizations/OrganizationController.cs)

### AI Service Patterns
- **Enhanced AI capabilities** with specialized agents (audit, compliance, general)
- **Multi-model support**: GitHub Models + custom deployment
- **Conversation threading** for multi-turn interactions
- **Tool calling** capabilities for complex workflows
- Contracts defined in [DoganConsult.AI.Application.Contracts/Services/IEnhancedAIService.cs](aspnet-core/src/DoganConsult.AI.Application.Contracts/Services/IEnhancedAIService.cs)

### Multi-Tenant Architecture
- **Tenant isolation** at database level
- **Organization-based tenancy** (each org = tenant)
- Services communicate via HTTP clients with tenant context

### API Gateway Routing
- **YARP reverse proxy** configuration
- Service endpoints: `/api/{service-name}/{resource}`
- Example: Organization API at `/api/organization/organizations`

## File Organization Rules

### New Service Creation
When adding services, follow the established pattern:
1. Use ABP CLI: `abp new DoganConsult.{ServiceName} -t microservice-service-pro`
2. Add to docker-compose.yml with dedicated database
3. Update migration scripts (`create-migrations.ps1`, `run-migrations.ps1`)
4. Configure in API Gateway routing

### Cross-Service Communication
- Use `HttpApi.Client` packages for typed service clients
- Configure service URLs in `appsettings.json`
- Handle authentication via Identity service tokens

## Critical Dependencies

- **ABP Framework 10.0** (microservice template)
- **OpenIddict** (authentication/authorization)
- **PostgreSQL** (Railway hosting, 7 instances)
- **Redis** (caching layer)
- **YARP** (API Gateway)
- **Entity Framework Core** (ORM)
- **Blazor Server** (UI framework)

## Debugging & Troubleshooting

### Common Issues
- **Service startup order**: Identity service must start before others
- **Database connections**: Each service has separate Railway PostgreSQL instance
- **Authentication**: All services depend on Identity service for auth
- **Migrations**: Run in sequence, Identity first

### Logs & Monitoring
- Service logs in individual `Logs/` directories
- **Request/response logging** configured in each HttpApi.Host
- **Performance monitoring** via ABP's built-in features