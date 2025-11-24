# ElectroHuila - Appointment Scheduling System

## Architecture Overview

This project has been migrated from MVC architecture to **Clean Architecture** following Domain-Driven Design (DDD) principles and implementing the CQRS (Command Query Responsibility Segregation) pattern.

## Project Structure

```
ElectroHuila.Solution/
│
├── src/
│   │
│   ├── 1. Core/
│   │   └── ElectroHuila.Domain/           # Domain layer - Business logic and entities
│   │       ├── Entities/
│   │       │   ├── Common/                # Base entities
│   │       │   ├── Appointments/          # Appointment domain entities
│   │       │   ├── Clients/              # Client domain entities
│   │       │   ├── Locations/            # Location domain entities
│   │       │   └── Security/             # Security domain entities
│   │       └── [Other domain concepts]
│   │
│   ├── 2. Application/
│   │   └── ElectroHuila.Application/      # Application layer - Use cases and business rules
│   │       ├── Features/                  # CQRS Commands and Queries
│   │       ├── DTOs/                     # Data Transfer Objects
│   │       ├── Contracts/                # Interface contracts
│   │       ├── Behaviors/                # Cross-cutting concerns (validation, logging)
│   │       └── Mappings/                 # AutoMapper profiles
│   │
│   ├── 3. Infrastructure/
│   │   └── ElectroHuila.Infrastructure/   # Infrastructure layer - External concerns
│   │       ├── Persistence/              # Database context and repositories
│   │       └── Services/                 # External services implementation
│   │
│   └── 4. Presentation/
│       └── ElectroHuila.WebApi/          # Presentation layer - API controllers
│           └── Controllers/              # REST API endpoints
│
└── [Old MVC structure] - To be removed after migration completion
```

## Key Features

### Clean Architecture Benefits
- **Separation of Concerns**: Each layer has a specific responsibility
- **Dependency Inversion**: Dependencies point inward toward the domain
- **Testability**: Easy to unit test business logic
- **Maintainability**: Changes in outer layers don't affect inner layers

### CQRS Implementation
- **Commands**: Handle write operations (Create, Update, Delete)
- **Queries**: Handle read operations (Get, List, Search)
- **Handlers**: Process commands and queries independently
- **Validation**: Built-in validation using FluentValidation

### Technologies Used
- **.NET 9.0**
- **Entity Framework Core 9.0**
- **MediatR** - For CQRS implementation
- **AutoMapper** - For object mapping
- **FluentValidation** - For request validation
- **SQL Server** - Database

## Getting Started

### Prerequisites
- .NET 9.0 SDK
- SQL Server (LocalDB or full instance)

### Running the Application

1. **Restore packages**:
   ```bash
   dotnet restore
   ```

2. **Build the solution**:
   ```bash
   dotnet build
   ```

3. **Update database connection string** in `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ElectroHuila;Trusted_Connection=true;MultipleActiveResultSets=true"
     }
   }
   ```

4. **Run the API**:
   ```bash
   cd src/4.\ Presentation/ElectroHuila.WebApi
   dotnet run
   ```

5. **Access Swagger UI**:
   Navigate to `https://localhost:5001/swagger` or `http://localhost:5000/swagger`

## API Endpoints

### Appointments
- `GET /api/appointments/{id}` - Get appointment by ID
- `POST /api/appointments` - Create new appointment

### Future Endpoints (To be implemented)
- Clients management
- Branches management
- Authentication
- User management

## Migration Status

✅ **Completed**:
- Domain layer with all entities
- Application layer with CQRS foundation
- Infrastructure layer with repositories
- WebAPI presentation layer
- Basic appointment operations

🚧 **In Progress**:
- Complete CQRS implementation for all entities
- Authentication and authorization
- Additional business rules
- Error handling middleware

📋 **Pending**:
- Complete migration of all business logic
- Integration tests
- Performance optimizations
- Security implementation

## Development Guidelines

### Adding New Features
1. **Domain**: Define entities and domain rules in the Domain layer
2. **Application**: Create Commands/Queries in the Application layer
3. **Infrastructure**: Implement repositories if needed
4. **Presentation**: Add controllers in the WebAPI layer

### Naming Conventions
- **Commands**: `{Action}{Entity}Command` (e.g., `CreateAppointmentCommand`)
- **Queries**: `Get{Entity}By{Criteria}Query` (e.g., `GetAppointmentByIdQuery`)
- **Handlers**: `{CommandOrQuery}Handler`
- **DTOs**: `{Entity}Dto`, `Create{Entity}Dto`, `Update{Entity}Dto`

## Contributing

When contributing to this project, please follow the established patterns and maintain the clean architecture principles.