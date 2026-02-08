namespace Trivia.Api.Storage;

public class TriviaTokenStorage : ITriviaTokenStorage
{
    private readonly Dictionary<string, string> _sessionTokens = new();

    public string? GetToken(string sessionId)
    {
        if (_sessionTokens.TryGetValue(sessionId, out var token))
        {
            return token;
        }

        return null;
    }

    public void SaveToken(string sessonId, string token)
    {
        _sessionTokens[sessonId] = token;
    }

    public void ClearToken(string sessionId)
    {
        _sessionTokens.Remove(sessionId);
    }
}