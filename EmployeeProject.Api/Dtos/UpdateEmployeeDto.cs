namespace EmployeeProject.Api.Dtos;

public record UpdateEmployeeDto(
    string Name,
    string Position,
    string Department,
    int Manager
);