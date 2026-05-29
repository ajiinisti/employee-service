using System.ComponentModel.DataAnnotations;
namespace EmployeeProject.Api.Dtos;

public record CreateEmployeeDto(
    [Required][StringLength(50)] string Name,
    [Required] string Position,
    [Required] string Department,
    [Range(0, int.MaxValue)] int Manager
);
