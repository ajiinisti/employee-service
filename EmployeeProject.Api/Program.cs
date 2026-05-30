using EmployeeProject.Api.Endpoints;
using EmployeeProject.Api.Data;
using EmployeeProject.Api.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddValidation();
builder.SetDatabase();

var app = builder.Build();

app.MapEmployeeEndpoints();
app.MigrateDatabase();
app.Run();