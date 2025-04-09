namespace Domain.Responses;

public class StatusResult : ResponseResult
{
}

public class StatusResult<T> : ResponseResult
{
    public T? Result { get; set; }
}