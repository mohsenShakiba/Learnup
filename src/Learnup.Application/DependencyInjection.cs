using System.Reflection;
using Learnup.Application.AiPipelines;
using Learnup.Application.Authentication;
using Learnup.Application.Mediation;
using Microsoft.Extensions.DependencyInjection;

namespace Learnup.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IMediator, Mediator>();
        services.AddScoped<IIdentityProvider, TestIdentityProvider>();

        services.AddAiProcessors(typeof(DependencyInjection).Assembly);
        
        services.AddMediatorHandlers(typeof(DependencyInjection).Assembly);

        return services;
    }

    private static IServiceCollection AddAiProcessors(
        this IServiceCollection services,
        params Assembly[] assemblies)
    {
        var processorTypes = assemblies
            .SelectMany(assembly => assembly.DefinedTypes)
            .Where(type => type is { IsAbstract: false, IsInterface: false })
            .Where(type => type.IsAssignableTo(typeof(IPipeline)));

        foreach (var processorType in processorTypes)
        {
            services.AddTransient(typeof(IPipeline), processorType.AsType());
        }

        return services;
    }

    private static IServiceCollection AddMediatorHandlers(
        this IServiceCollection services,
        params Assembly[] assemblies)
    {
        var implementations = assemblies
            .SelectMany(assembly => assembly.DefinedTypes)
            .Where(type => type is { IsAbstract: false, IsInterface: false })
            .Select(type => new
            {
                Implementation = type.AsType(),
                Services = type.ImplementedInterfaces
                    .Where(IsMediatorHandlerInterface)
                    .ToArray()
            })
            .Where(type => type.Services.Length > 0);

        foreach (var implementation in implementations)
        {
            foreach (var service in implementation.Services)
            {
                services.AddScoped(service, implementation.Implementation);
            }
        }

        return services;
    }

    private static bool IsMediatorHandlerInterface(Type type)
    {
        if (!type.IsGenericType)
        {
            return false;
        }

        var genericType = type.GetGenericTypeDefinition();
        return genericType == typeof(IRequestHandler<,>)
            || genericType == typeof(INotificationHandler<>);
    }
}
