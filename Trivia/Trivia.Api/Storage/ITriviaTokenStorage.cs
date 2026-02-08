namespace Trivia.Api.Storage;

public interface ITriviaTokenStorage
{
    string? GetToken(string sessionId);
    void SaveToken(string sessonId, string token);
    void ClearToken(string sessionId);
}