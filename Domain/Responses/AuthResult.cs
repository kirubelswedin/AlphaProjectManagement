namespace Domain.Responses;

public class AuthResult : ResponseResult
{
    public string? SuccessMessage { get; set; }
}

public class AuthResult<T> : ResponseResult
{
    public T? Result { get; set; }
}