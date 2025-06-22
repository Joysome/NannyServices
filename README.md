# Nanny.Admin

A .NET 8 Web API for managing nanny services built with Clean Architecture.

## Quick Start

### First Time Setup
1. Clone the repository
2. Copy `env.example` to `.env` (if empty or doesn't exist)
3. Start the application. For the fresh start - database migrations are mandatory:
   ```bash
   docker compose --profile migration up
   ```

### Normal Operation
When you have all migrations and configuration set up - you can launch the application with just:
```bash
docker compose up
```
- Starts immediately without waiting for migrations
- Good for development and testing

### Verify Installation
- **API**: http://localhost:8080
- **Swagger**: http://localhost:8080/swagger
- **Health**: http://localhost:8080/health

## Architecture Overview

### Clean Architecture Layers
- **API Layer**: Controllers, middleware, health checks
- **Infrastructure Layer**: Database context, repositories, migrations
- **Application Layer**: Business logic, services, DTOs, validation
- **Domain Layer**: Entities, business rules, enums

### Key Features
- **Error Handling**: Global exception middleware with structured responses
- **Health Checks**: Database connectivity monitoring
- **Validation**: FluentValidation for input validation
- **Logging**: Serilog with structured logging
- **Testing**: Unit tests for all layers

## 🔄 Migration Strategy

### With Database Migrations
```bash
docker compose --profile migration up
```
- Ensures database schema is up-to-date
- Recommended for first run and production

### Reset Database
```bash
docker compose down -v
docker compose --profile migration up
```

## 🔧 Configuration

### Environment Variables
| Variable | Default | Description |
|----------|---------|-------------|
| `DB_PASSWORD` | `YourStrong@Passw0rd` | SQL Server password |
| `DB_NAME` | `NannyServicesDb` | Database name |
| `API_PORT` | `8080` | API port |

- DB_PASSWORD: - SQL Server password. Default: YourStrong@Passw0rd
- DB_NAME: - Database name. Default: NannyServicesDb
- API_PORT:  - API port. Default: 8080

### Connection String
```
Server=sqlserver;Database={DB_NAME};User=sa;Password={DB_PASSWORD};TrustServerCertificate=True;
```

## Available Commands

```bash
# Start services
docker compose up
docker compose up -d

# With migrations
docker compose --profile migration up

# Stop services
docker compose down

# View API logs
docker compose logs api
``` 