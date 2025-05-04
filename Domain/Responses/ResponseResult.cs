namespace Domain.Responses;

public abstract class ResponseResult
{
    // Base class for service responses
    public bool Succeeded { get; set; }
    public int? StatusCode { get; set; }
    public string? Error { get; set; }
}