using Microsoft.AspNetCore.Http;

namespace Business.Interfaces;

public interface IFileUploadService
{
  Task<string> UploadFileAsync(IFormFile file, string directory);
  string GetDefaultImage(string entityType);
}