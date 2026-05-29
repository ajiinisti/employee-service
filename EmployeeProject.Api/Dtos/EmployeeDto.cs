namespace EmployeeProject.Api.Dtos;

public record EmployeeDto(
    int Id,
    string Name,
    string Position,
    string Department,
    int Manager
);
