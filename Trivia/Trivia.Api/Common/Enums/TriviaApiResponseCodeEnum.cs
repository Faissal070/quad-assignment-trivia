namespace Trivia.Api.Common.Enums;

public enum TriviaApiResponseCodeEnum
{
    Success = 0,
    NoResults = 1,
    InvalidParameter = 2,
    TokenNotFound = 3,
    TokenEmpty = 4,
    RateLimit = 5
}
