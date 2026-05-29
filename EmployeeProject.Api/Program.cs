using EmployeeProject.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapEmployeeEndpoints();
app.Run();