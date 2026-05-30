namespace EmployeeProject.Api.Models;

public class Employee
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Position { get; set; }
    public Department? Department { get; set; }
    public int DepartmentId { get; set; }
    public Employee? Manager { get; set; }
    public int? ManagerId { get; set; }
}
