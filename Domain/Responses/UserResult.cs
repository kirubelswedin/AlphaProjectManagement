namespace Domain.Responses;

public class UserResult : ResponseResult
{
    public string? Result { get; set; }
}

public class UserResult<T> : ResponseResult
{
    public T? Result { get; set; }
}