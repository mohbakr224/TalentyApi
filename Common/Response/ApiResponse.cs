namespace Common.Response;

public class ApiResponse<T>
{
    public T? Data { get; set; }

    public bool Success { get; set; }

    public string? Message { get; set; }

    public static ApiResponse<T> SuccessResponse(
        T data,
        string? message = null)
    {
        return new ApiResponse<T>
        {
            Data = data,
            Success = true,
            Message = message
        };
    }

    public static ApiResponse<T> FailureResponse(
        string message)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message
        };
    }
}