using EmployeeProject.Api.Dtos; 

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();


List<EmployeeDto> employees = new List<EmployeeDto>
{
    new EmployeeDto(1, "John Doe", "Software Engineer Lead", "IT", 0),
    new EmployeeDto(2, "Alice Johnson", "Project Manager Lead", "IT", 1),
    new EmployeeDto(3, "Bob Brown", "Backend Software Engineer", "IT", 1),
    new EmployeeDto(4, "Charlie Johnson", "Project Manager Senior", "IT", 2),
    new EmployeeDto(5, "Channel Johnson", "Project Manager Junior", "IT", 4)
};

app.MapGet("/employees", () => employees);
app.MapGet("/employees/{id}", (int id) => employees.FirstOrDefault(e => e.Id == id)).WithName("GetEmployeeById");
app.MapPost("/employees", (CreateEmployeeDto employee) =>
{
    EmployeeDto newEmployee = new EmployeeDto(
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

    EmployeeDto newEmployee = new EmployeeDto(
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

    var managerId = employee.Manager;
    var subordinates = employees.Where(e => e.Manager == employee.Id).ToList();
    foreach (var subordinate in subordinates)
    {
        var updatedSubordinate = subordinate with { Manager = managerId };
        employees[employees.IndexOf(subordinate)] = updatedSubordinate;
    }

    employees.Remove(employee);
    return Results.NoContent();
});

app.Run();
