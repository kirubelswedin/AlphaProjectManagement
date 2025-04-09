namespace Domain.Responses;

public class NotificationResult : ResponseResult
{
}

public class NotificationResult<T> : ResponseResult
{
    public T? Result { get; set; }
}