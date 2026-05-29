using EmployeeProject.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddValidation();

var app = builder.Build();

app.MapEmployeeEndpoints();
app.Run();