using Amazon.Runtime;
using Amazon.S3;
using Learnup.Application.AiPipelines;
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

        services.Configure<S3FileStorageOptions>(
            configuration.GetSection(S3FileStorageOptions.SectionName));
        services.Configure<JwtOptions>(
            configuration.GetSection(JwtOptions.SectionName));

        services.AddSingleton<IAmazonS3>(_ =>
        {
            var options = configuration
                .GetSection(S3FileStorageOptions.SectionName)
                .Get<S3FileStorageOptions>() ?? new S3FileStorageOptions();

            if (string.IsNullOrWhiteSpace(options.ServiceUrl))
            {
                throw new InvalidOperationException("S3 service URL is not configured.");
            }

            if (string.IsNullOrWhiteSpace(options.AccessKey) ||
                string.IsNullOrWhiteSpace(options.SecretKey))
            {
                throw new InvalidOperationException("S3 credentials are not configured.");
            }

            var clientConfig = new AmazonS3Config
            {
                ServiceURL = options.ServiceUrl,
                ForcePathStyle = options.ForcePathStyle
            };
            
            var credentials = new BasicAWSCredentials(options.AccessKey, options.SecretKey);
            
            return new AmazonS3Client(credentials, clientConfig);
        });

        services.AddScoped<IFileService, S3FileService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IOtpSender, ConsoleOtpSender>();
        services.AddScoped<IAiTextService, OpenAiTextService>();
        services.AddScoped<IVoiceProvider, KokoroVoiceProvider>();
        services.AddScoped<IVocabTranslationProvider, AiVocabTranslationProvider>();
        services.AddScoped<ITestProvider, AiTestProvider>();
        services.AddScoped<IGrammarLoader, GrammarLoader>();
        services.AddScoped<IVocabLoader, VocabLoader>();
        services.AddScoped<IStoryLoader, StoryLoader>();

        return services;
    }
}
