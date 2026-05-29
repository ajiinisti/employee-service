using EmployeeProject.Api.Dtos;
namespace EmployeeProject.Api.Utilities;
public record UtilitiesFunction
{
    public static bool EmployeeExists(int id, IEnumerable<EmployeeDto> employees) => id == 0 || employees.Any(e => e.Id == id);

    public static bool CreatesCycle(int employeeId, int managerId, IEnumerable<EmployeeDto> employees)
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

    public static bool IsValidManager(int managerId, int employeeId, IEnumerable<EmployeeDto> employees)
    {
        if (managerId == 0)
        {
            return true;
        }

        if (!EmployeeExists(managerId, employees))
        {
            return false;
        }

        if (employeeId != 0 && CreatesCycle(employeeId, managerId, employees))
        {
            return false;
        }

        return true;
    }

    public static List<EmployeeDto> ReassignSubordinates(int deletedEmployeeId, int newManagerId, List<EmployeeDto> employees)
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
}
