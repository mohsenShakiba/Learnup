using Learnup.Application.Authentication;
using Learnup.Application.Mediation;
using Learnup.Application.Persistence;
using Learnup.Application.Responses.Public.Ai;
using Microsoft.EntityFrameworkCore;

namespace Learnup.Application.Features.Public.Ai;

public sealed record GetAvailableTokenUsage : IRequest<TokenUsageResponse>;

internal sealed class GetAvailableTokenUsageHandler(
    ILearnupDbContext dbContext,
    IIdentityProvider identityProvider)
    : IRequestHandler<GetAvailableTokenUsage, TokenUsageResponse>
{
    public async Task<TokenUsageResponse> Handle(GetAvailableTokenUsage request, CancellationToken cancellationToken)
    {
        var usage = await dbContext.UserTokenUsages
            .AsNoTracking()
            .Where(entry => entry.UserId == identityProvider.UserId)
            .Select(entry => new
            {
                entry.TotalTokens,
                entry.AvailableTokens,
            })
            .FirstOrDefaultAsync(cancellationToken);

        return new TokenUsageResponse(usage?.AvailableTokens ?? 0, usage?.TotalTokens ?? 0);
    }

}
