using Microsoft.AspNetCore.Http;
using ASP.Constants;

namespace ASP.Helpers;

public static class FileUploadHelper
{
  public static async Task<string> UploadFileAsync(IFormFile file, string directory, IWebHostEnvironment env)
  {
    if (file == null || file.Length == 0)
      return string.Empty;

    // Combine paths and create directory if it doesn't exist
    var uploadFolder = Path.Combine(env.WebRootPath, UploadConstants.BaseUploadPath, directory);
    Directory.CreateDirectory(uploadFolder);

    // Create unique filename
    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
    var filePath = Path.Combine(uploadFolder, fileName);

    // Save file
    using (var stream = new FileStream(filePath, FileMode.Create))
    {
      await file.CopyToAsync(stream);
    }

    // Return relative path for web
    return $"/{UploadConstants.BaseUploadPath}/{directory}/{fileName}";
  }

  public static string GetDefaultImage(string entityType)
  {
    return entityType switch
    {
      UploadConstants.Directories.Projects => UploadConstants.DefaultImages.Project,
      UploadConstants.Directories.Members => UploadConstants.DefaultImages.Member,
      UploadConstants.Directories.Clients => UploadConstants.DefaultImages.Client,
      _ => string.Empty
    };
  }
}