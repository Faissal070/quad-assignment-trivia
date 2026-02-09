namespace Trivia.Api.Storage;

public class TokenStore : ITokenStore
{
    private Dictionary<Guid, string> _tokenByQuizId = new();

    public string? GetToken(Guid quizId)
    {
        return _tokenByQuizId.TryGetValue(quizId, out var token)
            ? token
            : null;
    }

    public void SaveToken(Guid guizId, string token)
    {
        _tokenByQuizId[guizId] = token;
    }

    public void clearQuizToken(Guid quizId)
    {
        _tokenByQuizId.Remove(quizId);
    }
}
