namespace EmployeeProject.Api.Dtos;

public record CreateEmployeeDto(
    string Name,
    string Position,
    string Department,
    int Manager
);
