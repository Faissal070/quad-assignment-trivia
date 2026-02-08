namespace Trivia.Api.Storage;

public interface ITriviaTokenStorage
{
    string? GetToken();
    void SetToken(string token);
    void RemoveToken();
}