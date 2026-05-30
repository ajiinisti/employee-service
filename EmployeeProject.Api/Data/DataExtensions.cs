using Microsoft.EntityFrameworkCore;
using EmployeeProject.Api.Models;
namespace EmployeeProject.Api.Data;
public static class DataExtensions
{
    public static void SetDatabase(this WebApplicationBuilder builder)
    {
        var connectionString = "Data Source=employee-store.db";
        builder.Services.AddSqlite<EmployeeStoreContext>(connectionString);
    }

    public static void MigrateDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<EmployeeStoreContext>();
        db.Database.Migrate();
        SeedDatabase(db);
    }

    private static void SeedDatabase(EmployeeStoreContext db)
    {
        if (!db.Set<Department>().Any())
        {
            db.Set<Department>().AddRange(
                new Department { Id = 1, Name = "IT" },
                new Department { Id = 2, Name = "HR" },
                new Department { Id = 3, Name = "Finance" }
            );
        }

        if (!db.Set<Employee>().Any())
        {
            db.Set<Employee>().AddRange(
                new Employee { Id = 1, Name = "Alice", Position = "Software Engineer Lead", DepartmentId = 1, ManagerId = null },
                new Employee { Id = 2, Name = "Bob", Position = "Senior Developer", DepartmentId = 2, ManagerId = 1 },
                new Employee { Id = 3, Name = "Charlie", Position = "Frontend Developer", DepartmentId = 3, ManagerId = 2 },
                new Employee { Id = 4, Name = "David", Position = "Project Manager", DepartmentId = 1, ManagerId = 1 },
                new Employee { Id = 5, Name = "Eve", Position = "QA Engineer", DepartmentId = 2, ManagerId = 1 }
            );
        }

        if (db.ChangeTracker.HasChanges())
        {
            db.SaveChanges();
        }
    }
}