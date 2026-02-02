using Trivia.Api.Models.Responses;

namespace Trivia.Api.Services
{
    public interface ITriviaService
    {
        public Task<TriviaQuestionsResult> GetQuestionsAsync(int amount);
    }
}
