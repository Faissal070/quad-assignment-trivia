namespace Trivia.Api.Storage;

public interface ITokenStore
{
    string? GetToken(Guid quizId);
    void SaveToken(Guid guizId, string token);
    void clearQuizToken(Guid quizId);
}
