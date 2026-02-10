namespace Trivia.Api.Services;

public interface ITriviaTokenService 
{
    Task<string?> GetOrCreateTokenAsync(Guid quizId);
    Task<bool> ResetTokenAsync(Guid quizId, string token);
    void ClearToken(Guid quizId);
}
    