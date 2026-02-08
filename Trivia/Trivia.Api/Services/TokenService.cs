using Trivia.Api.Client;
using Trivia.Api.Common.Enums;
using Trivia.Api.Storage;

namespace Trivia.Api.Services;

public class TokenService : ITokenService
{
    private readonly ITriviaTokenStorage _tokenStorage; 
    private readonly ITriviaApiClient _triviaApiClient;

    public TokenService(ITriviaApiClient triviaApiClient, ITriviaTokenStorage tokenStorage)
    {
        _tokenStorage = tokenStorage;
        _triviaApiClient = triviaApiClient;
    }

    public async Task<string?> GetTokenAsync()
    {
        var token = _tokenStorage.GetToken();

        if (!string.IsNullOrWhiteSpace(token))
        {
            return token;
        }

        var tokenResponse = await _triviaApiClient.FetchTokenAsync();

        if (tokenResponse.ResponseCode != (int)TriviaApiResponseCodeEnum.Success ||
            string.IsNullOrWhiteSpace(tokenResponse.Token))
        {
            return null;
        }   

        _tokenStorage.SetToken(tokenResponse.Token);
        return tokenResponse.Token;
    }

    public async Task HandleTokenErrorAsync(string token, int responseCode)
    {
        switch (responseCode)
        {
            case (int)TriviaApiResponseCodeEnum.TokenNotFound:
                _tokenStorage.RemoveToken();
                break;

            case (int)TriviaApiResponseCodeEnum.TokenEmpty:
                await ResetTokenAsync(token);
                break;

            default:
                break;
        }
    }

    private async Task ResetTokenAsync(string token)
    {
        var resetResponseCode = await _triviaApiClient.ResetTokenAsync(token);

        if (resetResponseCode.ResponseCode == (int)TriviaApiResponseCodeEnum.Success)
        {
            return;
        }

        _tokenStorage.RemoveToken();
    }
}