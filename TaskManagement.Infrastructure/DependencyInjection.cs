using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskManagement.Domain.Interfaces;
using TaskManagement.Infrastructure.Persistence;

namespace TaskManagement.Infrastructure;

public static class DependencyInjection
{
    // Extension method on IServiceCollection
    // This is how you add all Infrastructure services in one line
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
        );

        // Scoped means: one instance per HTTP request.
        // Every request gets its own UnitOfWork (and therefore
        // its own DbContext), which is exactly what we want.
        // All repositories in one request share the same DbContext,
        // which means they share the same transaction.
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
