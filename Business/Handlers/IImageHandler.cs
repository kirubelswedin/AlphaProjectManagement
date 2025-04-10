using Microsoft.AspNetCore.Http;

namespace Business.Handlers;

public interface IImageHandler
{
    Task<string?> SaveImageAsync(IFormFile file, string directory);
}