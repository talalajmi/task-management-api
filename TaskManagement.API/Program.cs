using Hangfire;
using Scalar.AspNetCore;
using TaskManagement.API.Hubs;
using TaskManagement.API.Middleware;
using TaskManagement.Application;
using TaskManagement.Infrastructure;
using TaskManagement.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins("http://localhost:8080")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

// app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "Task Management API";
        options.Theme = ScalarTheme.DeepSpace;
        options.AddHttpAuthentication(
            "Bearer",
            scheme =>
            {
                scheme.Token = "your-jwt-token-here";
            }
        );
    });
}

app.UseHangfireDashboard("/hangfire");

app.Services.GetRequiredService<IRecurringJobManager>()
    .AddOrUpdate<TaskCleanupService>(
        "overdue-task-check",
        service => service.LogOverdueTasksAsync(),
        Cron.Daily
    );

app.MapControllers();
app.MapHub<TaskHub>("/hubs/tasks");

app.Run();
