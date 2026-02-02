using System.Runtime.Serialization;

namespace Trivia.Api.Enums
{
    public enum TriviaApiResponseCodeEnum
    {
        [EnumMember(Value = "Success")]
        Success = 0,

        [EnumMember(Value = "No results")]
        NoResults = 1,

        [EnumMember(Value = "Invalid parameter")]
        InvalidParameter = 2,

        [EnumMember(Value = "Token not found")]
        TokenNotFound = 3,

        [EnumMember(Value = "Token empty")]
        TokenEmpty = 4,

        [EnumMember(Value = "Rate limit")]
        RateLimit = 5
    }
}
