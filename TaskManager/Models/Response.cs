namespace TaskManager.Models;

public class SuccessResponse<T>
{
    public string Success { get; } = "success";
    public T? Data { get; set; }
}

public class ErrorResponse
{
    public string Success { get; } = "error";
    public string Details { get; set; } = string.Empty;
}