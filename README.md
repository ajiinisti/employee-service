# Learning C# and .NET

This document is created to help learn C# and .NET through a simple sample project.

## Goals

- Learn basic C# syntax
- Understand .NET project structure
- Use minimal APIs in ASP.NET Core
- Manage data with DTOs
- Understand CRUD flow (Create, Read, Update, Delete) on .Net

## Topics

1. C# Basics
   - Variables, data types, and operators
   - Control flow: `if`, `switch`, `for`, `while`
   - Functions/methods and parameters
2. Object Oriented Programming (OOP)
   - Class, record, property, constructor
   - Immutability with `record`
3. .NET Project Structure
   - `.slnx` and `.csproj` files
   - `bin/`, `obj/`, `Properties/` folders
4. ASP.NET Core Minimal API
   - `WebApplication.CreateBuilder(args)`
   - `app.MapGet`, `app.MapPost`, `app.MapPut`, `app.MapDelete`
   - Route and URL parameters
5. DTO and data separation
   - `EmployeeDto`, `CreateEmployeeDto`, `UpdateEmployeeDto`
   - Separating data models from API request/response
6. Simple CRUD flow
   - Create a resource with `POST`
   - Read list and details with `GET`
   - Update a resource with `PUT`
   - Delete a resource with `DELETE`
7. Debugging and build
   - Run and test the application
   - Detect compile and runtime errors

## Learning Flow

1. Start by reading `Program.cs` to see how the API is built.
2. Study the DTO definitions in the `Dtos/` folder to understand the data model.
3. Follow each endpoint: how requests arrive, are processed, and return responses.
4. Review delete/update logic and manager/subordinate handling.
5. Understand validation rules for manager assignment and cycle prevention.
6. Run `dotnet build` and fix any errors.
7. Call endpoints using an HTTP client or `employee.http`.
8. Modify small parts of the code to understand how changes affect behavior.

## Current Application Flow

1. The application starts the builder and creates a `WebApplication`.
2. Employee data is temporarily stored in the `employees` list.
3. API endpoints handle requests:
   - `GET /employees` returns all employees
   - `GET /employees/{id}` returns employee details
   - `GET /employees/{id}/subordinates` returns direct reports
   - `GET /employees/{id}/manager` returns the direct manager
   - `GET /employees/{id}/hierarchy` returns the manager chain and direct reports
   - `POST /employees` creates a new employee
   - `PUT /employees/{id}` updates employee data and manager assignment
   - `DELETE /employees/{id}` deletes an employee and reassigns subordinates
4. When deleting an employee, subordinates are reassigned to the deleted employee's manager.
5. The API response is returned in JSON format.

## Validation and Error Flow

- `POST /employees` and `PUT /employees/{id}` validate the manager ID.
- A manager ID of `0` means no manager.
- An employee cannot be their own manager.
- Manager assignment is checked to avoid cycles in the reporting chain.
- Invalid requests return `BadRequest`, while missing resources return `NotFound`.

## Notes

- This file is used to record key concepts and the learning flow.
- Focus on understanding concepts, not only copying code.
 
## Project Status & Roadmap

- **Current state:** a simple in-memory minimal API demonstrating CRUD and manager/subordinate handling.
- **Planned extensions:**
   - Implementing a database reinforces project structure and data modeling.
   - Moving logic into services and repositories teaches clean architecture and testability.
   - Writing tests improves debugging skills and confidence in changes.

## Notes for running and testing locally

- To build without producing an `apphost` (helps when the exe is locked):

```bash
dotnet build /p:UseAppHost=false
```

- To run the API locally:

```bash
dotnet run --project EmployeeProject.Api
```

