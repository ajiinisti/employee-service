using EmployeeProject.Api.Dtos;
using EmployeeProject.Api.Utilities;

namespace EmployeeProject.Api.Endpoints;

public static class EmployeeEndpoints
{
    private static readonly List<EmployeeDto> employees = new()
    {
        new EmployeeDto(1, "John Doe", "Software Engineer Lead", "IT", 0),
        new EmployeeDto(2, "Alice Johnson", "Project Manager Lead", "IT", 1),
        new EmployeeDto(3, "Bob Brown", "Backend Software Engineer", "IT", 1),
        new EmployeeDto(4, "Charlie Johnson", "Project Manager Senior", "IT", 2),
        new EmployeeDto(5, "Channel Johnson", "Project Manager Junior", "IT", 4)
    };

    public static void MapEmployeeEndpoints(this WebApplication app)
    {

        var group = app.MapGroup("/employees").WithTags("Employees");

        group.MapGet("", () => employees);

        group.MapGet("/{id}", (int id) =>
        {
            var employee = employees.FirstOrDefault(e => e.Id == id);
            return employee is null ? Results.NotFound() : Results.Ok(employee);
        }).WithName("GetEmployeeById");

        group.MapGet("/{id}/subordinates", (int id) =>
        {
            var employee = employees.FirstOrDefault(e => e.Id == id);
            if (employee is null)
            {
                return Results.NotFound();
            }

            var subordinates = employees.Where(e => e.Manager == id).ToList();
            return Results.Ok(subordinates);
        });
        group.MapGet("/{id}/manager", (int id) =>
        {
            var employee = employees.FirstOrDefault(e => e.Id == id);
            if (employee is null)
            {
                return Results.NotFound();
            }

            if (employee.Manager == 0)
            {
                return Results.NoContent();
            }

            var manager = employees.FirstOrDefault(e => e.Id == employee.Manager);
            return manager is null ? Results.NotFound() : Results.Ok(manager);
        });
        group.MapGet("/{id}/hierarchy", (int id) =>
        {
            var employee = employees.FirstOrDefault(e => e.Id == id);
            if (employee is null)
            {
                return Results.NotFound();
            }

            var managerChain = new List<EmployeeDto>();
            var current = employee;
            while (current.Manager != 0)
            {
                var manager = employees.FirstOrDefault(e => e.Id == current.Manager);
                if (manager is null)
                {
                    break;
                }

                managerChain.Add(manager);
                current = manager;
                if (managerChain.Count > employees.Count)
                {
                    break;
                }
            }

            var directSubordinates = employees.Where(e => e.Manager == id).ToList();
            return Results.Ok(new { Employee = employee, ManagerChain = managerChain, DirectSubordinates = directSubordinates });
        });
        group.MapPost("", (CreateEmployeeDto employee) =>
        {
            if (!UtilitiesFunction.IsValidManager(employee.Manager, 0, employees))
            {
                return Results.BadRequest(new { Error = "Invalid manager id." });
            }

            EmployeeDto newEmployee = new(
                Id: employees.Count + 1,
                Name: employee.Name,
                Position: employee.Position,
                Department: employee.Department,
                Manager: employee.Manager
            );

            employees.Add(newEmployee);
            return Results.CreatedAtRoute("GetEmployeeById", new { id = newEmployee.Id }, newEmployee);
        });
        group.MapPut("/{id}", (int id, UpdateEmployeeDto updatedEmployee) =>
        {
            var employee = employees.FirstOrDefault(e => e.Id == id);
            if (employee is null)
            {
                return Results.NotFound();
            }

            if (updatedEmployee.Manager == id)
            {
                return Results.BadRequest(new { Error = "Employee cannot be their own manager." });
            }

            if (!UtilitiesFunction.IsValidManager(updatedEmployee.Manager, id, employees))
            {
                return Results.BadRequest(new { Error = "Invalid manager id or manager chain would create a cycle." });
            }

            EmployeeDto newEmployee = new(
                Id: employee.Id,
                Name: updatedEmployee.Name,
                Position: updatedEmployee.Position,
                Department: updatedEmployee.Department,
                Manager: updatedEmployee.Manager
            );

            employees[employees.IndexOf(employee)] = newEmployee;
            return Results.NoContent();
        });
        group.MapDelete("/{id}", (int id) =>
        {
            var employee = employees.FirstOrDefault(e => e.Id == id);
            if (employee is null)
            {
                return Results.NotFound();
            }

            var reassigned = UtilitiesFunction.ReassignSubordinates(employee.Id, employee.Manager, employees);
            employees.Remove(employee);
            return Results.Ok(new { Deleted = employee, ReassignedSubordinates = reassigned });
        });

    }
}
