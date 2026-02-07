# PolicyManagement Application

## Overview
PolicyManagement is an ASP.NET Core Web API for managing insurance policies, users, and enrollments with robust authentication, role-based access, and logging. It uses Entity Framework Core with PostgreSQL and supports JWT authentication and Serilog file logging.

## Features
- User registration and login (JWT authentication)
- Role-based access (Admin, User)
- CRUD operations for policies
- Policy enrollment requests and admin approval/rejection
- Global error handling and response formatting
- Logging with Serilog (file-based)
- Swagger UI with JWT support

## Project Structure
- **Controllers/**: API endpoints for Auth, User, Policy, Enrollment
- **DTOs/**: Data transfer objects for requests/responses
- **Entities/**: Database models (User, Policy, PolicyEnrollment)
- **Repositories/**: Data access logic
- **Services/**: Business logic
- **Filters/**: Exception and response filters
- **Scripts/**: SQL scripts for DB setup

## Getting Started
1. Install dependencies: `dotnet restore`
2. Update `appsettings.json` with your PostgreSQL connection string and JWT settings
3. Build and run: `dotnet run`
4. Access Swagger UI at `http://localhost:5115/swagger`

## Logging
- Logs are written to `Logs/policymanagement.log` (Serilog)

## Authentication
- Register or login to receive a JWT token
- Use the token in the `Authorization: Bearer <token>` header for protected endpoints

## API Endpoints
See `APIs.md` for detailed documentation of all endpoints.