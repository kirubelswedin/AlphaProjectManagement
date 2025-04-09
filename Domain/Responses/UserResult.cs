namespace Domain.Responses;

public class UserResult : ResponseResult
{
}

public class UserResult<T> : ResponseResult
{
    public T? Result { get; set; }
}