using Microsoft.Extensions.DependencyInjection;

namespace Learnup.Application.Mediation;

internal sealed class Mediator(IServiceProvider serviceProvider) : IMediator
{
    public Task<TResponse> Send<TResponse>(
        IRequest<TResponse> request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var requestType = request.GetType();
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));
        var handler = serviceProvider.GetService(handlerType)
            ?? throw new InvalidOperationException($"No request handler registered for {requestType.FullName}.");

        RequestHandlerDelegate<TResponse> handlerDelegate = () =>
        {
            var handleMethod = handlerType.GetMethod(nameof(IRequestHandler<IRequest<TResponse>, TResponse>.Handle))!;
            return (Task<TResponse>)handleMethod.Invoke(handler, [request, cancellationToken])!;
        };

        var behaviorType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, typeof(TResponse));
        var behaviors = serviceProvider.GetServices(behaviorType).Reverse().ToArray();

        foreach (var behavior in behaviors)
        {
            var next = handlerDelegate;
            handlerDelegate = () =>
            {
                var handleMethod = behaviorType.GetMethod(nameof(IPipelineBehavior<IRequest<TResponse>, TResponse>.Handle))!;
                return (Task<TResponse>)handleMethod.Invoke(behavior, [request, next, cancellationToken])!;
            };
        }

        return handlerDelegate();
    }

    public Task Send(IRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return Send<Unit>(request, cancellationToken);
    }

    public async Task Publish<TNotification>(
        TNotification notification,
        CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        ArgumentNullException.ThrowIfNull(notification);

        var handlers = serviceProvider.GetServices<INotificationHandler<TNotification>>();

        foreach (var handler in handlers)
        {
            await handler.Handle(notification, cancellationToken);
        }
    }
}
