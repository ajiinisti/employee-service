using EmployeeProject.Api.Dtos;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

List<EmployeeDto> employees = new()
{
    new EmployeeDto(1, "John Doe", "Software Engineer Lead", "IT", 0),
    new EmployeeDto(2, "Alice Johnson", "Project Manager Lead", "IT", 1),
    new EmployeeDto(3, "Bob Brown", "Backend Software Engineer", "IT", 1),
    new EmployeeDto(4, "Charlie Johnson", "Project Manager Senior", "IT", 2),
    new EmployeeDto(5, "Channel Johnson", "Project Manager Junior", "IT", 4)
};

app.MapGet("/employees", () => employees);
app.MapGet("/employees/{id}", (int id) => employees.FirstOrDefault(e => e.Id == id)).WithName("GetEmployeeById");
app.MapGet("/employees/{id}/subordinates", (int id) =>
{
    var employee = employees.FirstOrDefault(e => e.Id == id);
    if (employee is null)
    {
        return Results.NotFound();
    }

    var subordinates = employees.Where(e => e.Manager == id).ToList();
    return Results.Ok(subordinates);
});
app.MapGet("/employees/{id}/manager", (int id) =>
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
app.MapGet("/employees/{id}/hierarchy", (int id) =>
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
app.MapPost("/employees", (CreateEmployeeDto employee) =>
{
    if (!IsValidManager(employee.Manager, 0))
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
app.MapPut("/employees/{id}", (int id, UpdateEmployeeDto updatedEmployee) =>
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

    if (!IsValidManager(updatedEmployee.Manager, id))
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
app.MapDelete("/employees/{id}", (int id) =>
{
    var employee = employees.FirstOrDefault(e => e.Id == id);
    if (employee is null)
    {
        return Results.NotFound();
    }

    var reassigned = ReassignSubordinates(employee.Id, employee.Manager);
    employees.Remove(employee);
    return Results.Ok(new { Deleted = employee, ReassignedSubordinates = reassigned });
});

app.Run();

bool EmployeeExists(int id) => id == 0 || employees.Any(e => e.Id == id);

bool CreatesCycle(int employeeId, int managerId)
{
    var currentManagerId = managerId;
    while (currentManagerId != 0)
    {
        if (currentManagerId == employeeId)
        {
            return true;
        }

        var manager = employees.FirstOrDefault(e => e.Id == currentManagerId);
        if (manager is null)
        {
            break;
        }

        currentManagerId = manager.Manager;
    }

    return false;
}

bool IsValidManager(int managerId, int employeeId)
{
    if (managerId == 0)
    {
        return true;
    }

    if (!EmployeeExists(managerId))
    {
        return false;
    }

    if (employeeId != 0 && CreatesCycle(employeeId, managerId))
    {
        return false;
    }

    return true;
}

List<EmployeeDto> ReassignSubordinates(int deletedEmployeeId, int newManagerId)
{
    var subordinates = employees.Where(e => e.Manager == deletedEmployeeId).ToList();
    var updatedSubordinates = new List<EmployeeDto>();

    foreach (var subordinate in subordinates)
    {
        var updatedSubordinate = subordinate with { Manager = newManagerId };
        employees[employees.IndexOf(subordinate)] = updatedSubordinate;
        updatedSubordinates.Add(updatedSubordinate);
    }

    return updatedSubordinates;
}
