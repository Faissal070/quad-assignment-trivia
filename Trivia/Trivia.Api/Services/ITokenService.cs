namespace Trivia.Api.Services;

public interface ITokenService 
{
    Task<string?> GetTokenAsync(string sessionId);
    Task HandleTokenResponseAsync(string sessionId, string token, int responseCode);
}