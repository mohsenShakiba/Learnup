using Learnup.Application.AiPipelines;
using Learnup.Application.ExternalServices;
using Learnup.Application.Persistence;
using Learnup.Infrastructure.ExternalService;
using Learnup.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Learnup.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PostgresSQL");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Database connection string is not configured.");
        }

        services.AddDbContext<LearnupDbContext>(options =>
            options.UseNpgsql(connectionString));
        services.AddScoped<ILearnupDbContext>(provider =>
            provider.GetRequiredService<LearnupDbContext>());

        services.AddHttpClient();
        services.AddScoped<IAiTextService, OpenAiTextService>();
        services.AddScoped<IVoiceProvider, KokoroVoiceProvider>();
        services.AddScoped<IVocabTranslationProvider, AiVocabTranslationProvider>();
        services.AddScoped<IVocabTestProvider, AiVocabTestProvider>();
        services.AddScoped<IGrammarTestProvider, AiGrammarTestProvider>();
        services.AddScoped<IGrammarLoader, GrammarLoader>();
        services.AddScoped<IVocabLoader, VocabLoader>();
        services.AddScoped<IStoryLoader, StoryLoader>();

        return services;
    }
}
