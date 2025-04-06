
using Business.Interfaces;
using ASP.Helpers;

namespace ASP.Services;

public class FileUploadService(IWebHostEnvironment env) : IFileUploadService
{
  public async Task<string> UploadFileAsync(IFormFile file, string directory)
  {
    return await FileUploadHelper.UploadFileAsync(file, directory, env);
  }

  public string GetDefaultImage(string entityType)
  {
    return FileUploadHelper.GetDefaultImage(entityType);
  }
}