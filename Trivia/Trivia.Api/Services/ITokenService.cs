namespace Trivia.Api.Services;

public interface ITokenService 
{
    Task<string?> GetTokenAsync();
    Task HandleTokenErrorAsync(string token, int responseCode);
}