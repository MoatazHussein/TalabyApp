namespace Talaby.Application.Common;

public class ApiResponse<T>
{
    public bool IsSuccess { get; private set; }
    public T? Data { get; private set; }
    public string? Message { get; private set; }
    public IReadOnlyList<FieldError>? Errors { get; private set; }
    public string? ErrorCode { get; private set; }

    private ApiResponse() { }

    public static ApiResponse<T> Success(T? data, string? message = null) =>
        new() { IsSuccess = true, Data = data, Message = message };

    public static ApiResponse<T> Fail(string message, string? errorCode = null, IReadOnlyList<FieldError>? errors = null) =>
        new() { IsSuccess = false, Message = message, ErrorCode = errorCode, Errors = errors };
}

public static class ApiResponse
{
    public static ApiResponse<object?> Success(string? message = null) =>
        ApiResponse<object?>.Success(null, message);

    public static ApiResponse<object?> Fail(string message, string? errorCode = null, IReadOnlyList<FieldError>? errors = null) =>
        ApiResponse<object?>.Fail(message, errorCode, errors);
}
