namespace Learnup.Application.Mediation;

public interface IRequest<out TResponse>
{
}

public interface IRequest : IRequest<Unit>
{
}
