namespace Trivia.Api.Common.Results;

public sealed class Result<T>
{
    public bool IsSuccess { get; }
    public string Message { get; }
    public T? Data { get; }

    private Result(bool isSuccess, T? data, string message)
    {
        IsSuccess = isSuccess;
        Data = data;
        Message = message;
    }

    public static Result<T> Success(T data, string message)
        => new(true, data, message);

    public static Result<T> Failure(string message)
        => new(false, default, message);
}