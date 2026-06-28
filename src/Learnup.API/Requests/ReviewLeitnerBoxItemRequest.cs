using Learnup.Application.Features.Public.LeitnerBoxes;
using Learnup.Domain.AggregateRoots.LeitnerBoxes;

namespace Learnup.API.Requests;

public record ReviewLeitnerBoxItemRequest(AnswerQuality AnswerQuality);
