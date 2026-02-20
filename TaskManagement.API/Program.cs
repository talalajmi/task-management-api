using TaskManagement.API.Hubs;
using TaskManagement.API.Middleware;
using TaskManagement.Application;
using TaskManagement.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddApplication();

// One clean line instead of manually registering everything
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();

// SignalR is built into ASP.NET Core — no extra package needed
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Exception middleware must be FIRST in the pipeline
// so it can catch exceptions from all other middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

// ORDER MATTERS — Authentication must come before Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Map the Hub to a URL — clients connect to this endpoint
app.MapHub<TaskHub>("/hubs/tasks");

app.Run();
