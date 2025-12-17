# DoganConsult Platform - Multi-Service AI & Consulting Platform

A comprehensive enterprise platform built with ABP Framework, featuring AI-powered consulting services, organization management, and multi-tenant architecture.

## 🚀 Project Overview

DoganConsult Platform is a modern, scalable microservices architecture that provides:

- **AI-Powered Consulting Services** - Multi-model AI integration with specialized consulting agents
- **Organization Management** - Complete CRUD operations for managing client organizations
- **Multi-Tenant Architecture** - Secure, isolated environments for different organizations
- **Professional UI** - Modern Blazor Server UI with responsive design
- **Enterprise Security** - OAuth2/OpenID Connect authentication and authorization

## 🏗️ Architecture

### Microservices

1. **DoganConsult.AI** - AI Services with OpenAI integration
   - Multi-model support (GPT-4, Claude, etc.)
   - Specialized consulting agents (Audit, Compliance, General)
   - Redis caching for performance
   - Tool calling capabilities

2. **DoganConsult.Organization** - Organization Management
   - CRUD operations for organizations
   - Business sector and regulatory requirements tracking
   - Contact information management
   - Status and type categorization

3. **DoganConsult.Web** - Main UI Portal
   - Blazor Server with ABP Framework
   - Professional responsive design
   - SweetAlert2 notifications
   - Role-based access control

4. **DoganConsult.Identity** - Authentication & Authorization
   - OpenID Connect provider
   - User and role management
   - Multi-tenant support

5. **DoganConsult.Document** - Document Management (Planned)
6. **DoganConsult.Audit** - Audit Services (Planned)
7. **DoganConsult.UserProfile** - User Profile Management (Planned)
8. **DoganConsult.Workspace** - Workspace Management (Planned)

## 🛠️ Technology Stack

### Backend
- **.NET 10** - Latest .NET framework
- **ABP Framework** - Modular application framework
- **Entity Framework Core** - ORM with PostgreSQL
- **Redis** - Caching and session storage
- **OpenAI SDK** - AI integration
- **Serilog** - Structured logging

### Frontend
- **Blazor Server** - Interactive web UI framework
- **Bootstrap 5** - Responsive CSS framework
- **SweetAlert2** - Beautiful, responsive notifications
- **Font Awesome** - Professional icons

### Database
- **PostgreSQL** - Primary database (Railway hosting)
- **Entity Framework Core Migrations** - Database versioning

### DevOps & Deployment
- **Docker & Docker Compose** - Containerization
- **Railway.app** - Cloud PostgreSQL hosting
- **Nginx** - Reverse proxy and load balancing
- **Systemd** - Service management

## 🚀 Getting Started

### Prerequisites
- .NET 10 SDK
- PostgreSQL database
- Redis server
- Git

### Environment Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/[username]/doganconsult-platform.git
   cd doganconsult-platform
   ```

2. **Database Setup**
   - Update connection strings in `appsettings.json` files
   - Run database migrations:
   ```bash
   cd aspnet-core/src/DoganConsult.Web.DbMigrator
   dotnet run
   
   cd ../DoganConsult.Organization.DbMigrator
   dotnet run
   
   cd ../DoganConsult.Identity.DbMigrator
   dotnet run
   ```

3. **Start Services**
   ```bash
   # Start Organization API
   cd aspnet-core/src/DoganConsult.Organization.HttpApi.Host
   dotnet run
   
   # Start AI Service
   cd ../DoganConsult.AI.HttpApi.Host
   dotnet run
   
   # Start Web UI
   cd ../DoganConsult.Web.Blazor
   dotnet run
   ```

### Docker Setup (Alternative)

```bash
docker-compose up -d
```

## 🎯 Features

### Organization Management
- **Professional Form Validation** - Client-side and server-side validation
- **Rich UI Components** - Professional cards, modals, and input groups
- **Comprehensive Data Model** - Code, name, type, sector, location, contacts
- **Status Tracking** - Active, Pilot, Trial, Inactive status management
- **Search & Pagination** - Efficient data browsing
- **Export Capabilities** - Data export functionality

### AI Integration
- **Multi-Model Support** - OpenAI GPT models integration
- **Specialized Agents** - Domain-specific AI agents for consulting
- **Caching** - Redis-based response caching for performance
- **Tool Calling** - Function calling for business-specific operations
- **Context Management** - Conversation history and context preservation

### Security & Authentication
- **OpenID Connect** - Industry-standard authentication
- **Multi-Tenant** - Organization-level data isolation
- **Role-Based Access** - Granular permission management
- **HTTPS Everywhere** - Secure communication
- **CSRF Protection** - Cross-site request forgery protection

## 📁 Project Structure

```
doganconsult-platform/
├── aspnet-core/
│   ├── src/
│   │   ├── DoganConsult.AI.*/               # AI Services
│   │   ├── DoganConsult.Organization.*/     # Organization Management
│   │   ├── DoganConsult.Web.*/              # Web Portal
│   │   ├── DoganConsult.Identity.*/         # Identity Services
│   │   └── DoganConsult.[Service].*/        # Other Services
│   ├── test/                                # Unit & Integration Tests
│   └── deployment/                          # Deployment Scripts
├── docker-compose.yml                       # Docker Configuration
├── README.md                               # This file
└── .gitignore                              # Git ignore rules
```

## 🔧 Configuration

### Application Settings
Key configuration files:
- `aspnet-core/src/DoganConsult.Web.Blazor/appsettings.json`
- `aspnet-core/src/DoganConsult.Organization.HttpApi.Host/appsettings.json`
- `aspnet-core/src/DoganConsult.AI.HttpApi.Host/appsettings.json`

### Environment Variables
Required environment variables:
- `OPENAI_API_KEY` - OpenAI API key for AI services
- `REDIS_CONNECTION_STRING` - Redis connection string
- `DATABASE_CONNECTION_STRING` - PostgreSQL connection string

## 🚀 Deployment

### Railway.app Deployment
1. Create Railway project
2. Add PostgreSQL service
3. Configure environment variables
4. Deploy using provided scripts

### Docker Deployment
```bash
docker-compose -f docker-compose.yml up -d
```

### Manual Deployment
See `DEPLOYMENT.md` for detailed deployment instructions.

## 📊 Service Endpoints

- **Web Portal**: https://localhost:44373
- **Organization API**: https://localhost:44337
- **AI Services API**: https://localhost:44331
- **Identity Server**: https://localhost:44300

## 🧪 Testing

Run tests with:
```bash
cd aspnet-core
dotnet test
```

## 📝 API Documentation

API documentation is available via Swagger UI:
- Organization API: https://localhost:44337/swagger
- AI Services API: https://localhost:44331/swagger

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🔗 Links

- [ABP Framework Documentation](https://docs.abp.io/)
- [Blazor Documentation](https://docs.microsoft.com/en-us/aspnet/core/blazor/)
- [OpenAI API Documentation](https://platform.openai.com/docs/)

## 💬 Support

For support and questions:
- Create an issue in this repository
- Contact: [Your Contact Information]

---

**Built with ❤️ using ABP Framework and .NET 10**