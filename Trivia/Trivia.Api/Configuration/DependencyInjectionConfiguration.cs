using Trivia.Api.Client;
using Trivia.Api.Services;
using Trivia.Api.Storage;

namespace Trivia.Api.Configuration;

public class DependencyInjectionConfiguration
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        //Configure logging(optional)

        services.Configure<TriviaApiSettings>(configuration.GetSection("TriviaApi"));

        //HttpClient
        services.AddHttpClient<ITriviaApiClient, TriviaApiClient>();

        //Scoped services
        services.AddScoped<ITriviaQuestionService, TriviaQuestionService>();
        services.AddScoped<ITriviaAnswerService, TriviaAnswerService>();
        services.AddScoped<ITriviaTokenService, TriviaTokenService>();

        //In-memory storage services
        services.AddSingleton<ICorrectAnswerStore, CorrectAnswerStore>();
        services.AddSingleton<ITokenStore, TokenStore>();
    }
}
