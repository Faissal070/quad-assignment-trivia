namespace Trivia.Api.Storage;

public class TriviaTokenStorage : ITriviaTokenStorage
{
    private string? _token;

    public string? GetToken()
    {
       return _token;
    }

    public void SetToken(string token)
    {
        _token = token;
    }

    public void RemoveToken()
    {
        _token = null; 
    }
}