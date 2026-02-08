using Trivia.Api.Client;
using Trivia.Api.Services;
using Trivia.Api.Storage;

namespace Trivia.Api.Configuration;

public class DependencyInjectionConfiguration
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<TriviaApiSettings>(configuration.GetSection("TriviaApi"));


        //HttpClient
        services.AddHttpClient<ITriviaApiClient, TriviaApiClient>();

        //Scoped services
        services.AddScoped<ITriviaService, TriviaService>();
        services.AddScoped<ITokenService, TokenService>();

        //In-memory storage services
        services.AddScoped<ICorrectAnswerStore, CorrectAnswerStore>();
        services.AddScoped<ITriviaTokenStorage, TriviaTokenStorage>();
    }
}