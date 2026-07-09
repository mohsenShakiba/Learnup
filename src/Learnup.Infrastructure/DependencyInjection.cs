using Amazon.Runtime;
using Amazon.S3;
using Learnup.Application.Authentication;
using Learnup.Application.ExternalServices;
using Learnup.Application.Persistence;
using Learnup.Infrastructure.Authentication;
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

        services.Configure<FileStorageOptions>(
            configuration.GetSection(FileStorageOptions.SectionName));
        services.Configure<S3FileStorageOptions>(
            configuration.GetSection(S3FileStorageOptions.SectionName));
        services.Configure<OsFileStorageOptions>(
            configuration.GetSection(OsFileStorageOptions.SectionName));
        services.Configure<JwtOptions>(
            configuration.GetSection(JwtOptions.SectionName));


        services.AddScoped<IFileService, OsFileService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IOtpSender, ConsoleOtpSender>();
        services.AddScoped<IAiService, AiService>();
        services.AddScoped<IVoiceProvider, ElevenLabsVoiceProvider>();
        services.AddScoped<GrammarLoader>();
        services.AddScoped<VocabLoader>();
        services.AddScoped<StoryLoader>();
        services.AddScoped<LessonGrammarLoader>();
        services.AddScoped<PlacementTestLoader>();

        return services;
    }

}
