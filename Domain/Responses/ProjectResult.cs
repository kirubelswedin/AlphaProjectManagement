namespace Domain.Responses;

public class ProjectResult : ResponseResult
{
}

public class ProjectResult<T> : ResponseResult
{
    public T? Result { get; set; }
}