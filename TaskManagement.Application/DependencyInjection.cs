using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TaskManagement.Application.Common.Behaviors;

namespace TaskManagement.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);

            // Register the validation behavior in the MediatR pipeline
            // This runs BEFORE every handler automatically
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        // Scans the assembly and registers ALL validators automatically
        // Just like MediatR handlers — add a new validator file and it works
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}
