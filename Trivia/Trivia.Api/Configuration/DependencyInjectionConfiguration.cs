using Trivia.Api.Services;
using Trivia.Api.Storage;

namespace Trivia.Api.Configuration
{
    public class DependencyInjectionConfiguration
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<TriviaApiSettings>(configuration.GetSection("TriviaApi"));

            services.AddHttpClient<ITriviaService, TriviaService>();
            services.AddSingleton<ICorrectAnswerStore, CorrectAnswerStore>();
        }   
    }
}
