using EmployeeProject.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeProject.Api.Data;

public class EmployeeStoreContext(DbContextOptions<EmployeeStoreContext> options) : DbContext(options)
{
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Department> Departments => Set<Department>();
}