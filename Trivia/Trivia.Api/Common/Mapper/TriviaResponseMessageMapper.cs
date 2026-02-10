using Trivia.Api.Common.Enums;

namespace Trivia.Api.Common.Mapper;

public static class TriviaResponseMessageMapper
{
    public static string ToPublicMessage(int responseCode)
    {
        return responseCode switch
        {
            (int)TriviaApiResponseCodeEnum.Success =>
                "Questions fetched successfully.",

            (int)TriviaApiResponseCodeEnum.NoResults =>
                "No questions available for the selected criteria.",

            (int)TriviaApiResponseCodeEnum.RateLimit =>
                "Too many requests. Please try again later.",

            (int)TriviaApiResponseCodeEnum.TokenNotFound =>
                "Session expired. Please start new session.",

            _ =>
                "Unable to fetch questions, try again later"
        };
    }
}
