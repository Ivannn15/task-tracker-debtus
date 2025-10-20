namespace TaskTracker.Infrastructure.Extensions;

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskTracker.Application.Abstractions;
using TaskTracker.Infrastructure.Persistence;
using TaskTracker.Infrastructure.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("TaskTracker")
                               ?? "Data Source=tasktracker.db";

        services.AddDbContext<TaskTrackerDbContext>(options =>
        {
            options.UseSqlite(connectionString);
        });

        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<ITaskGroupService, TaskGroupService>();
        services.AddScoped<ITaskService, TaskService>();

        return services;
    }

    public static async Task EnsureDatabaseCreatedAsync(this IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TaskTrackerDbContext>();
        await context.Database.EnsureCreatedAsync(cancellationToken);
    }
}
