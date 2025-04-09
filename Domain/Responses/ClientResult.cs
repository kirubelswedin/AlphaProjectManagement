namespace Domain.Responses;

public class ClientResult : ResponseResult
{
}

public class ClientResult<T> : ResponseResult
{
    public T? Result { get; set; }
}