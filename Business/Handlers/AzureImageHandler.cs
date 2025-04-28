using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Business.Handlers;

public class AzureImageHandler : IImageHandler
{
    private readonly BlobContainerClient _containerClient;
    private readonly IConfiguration _configuration;
    private readonly string _baseUrl;

    public AzureImageHandler(IConfiguration configuration)
    {
        _configuration = configuration;
        var connectionstring = _configuration["AzureStorageAccount:ConnectionString"];
        var containerName = _configuration["AzureStorageAccount:ContainerName"];
        _baseUrl = _configuration["AzureStorageAccount:BaseUrl"] ?? "";

        _containerClient = new BlobContainerClient(connectionstring, containerName);
        _containerClient.CreateIfNotExists();
    }

    public async Task<string?> SaveImageAsync(IFormFile? file, string directory)
    {
        if (file == null || file.Length == 0)
            return null!;

        var extension = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid()}{extension}";

        var blobClient = _containerClient.GetBlobClient(fileName);
        var contentType = file.ContentType;
        var blobHttpHeader = new BlobHttpHeaders
        {
            ContentType = contentType
        };

        await using var stream = file.OpenReadStream();
        await blobClient.UploadAsync(stream, new BlobUploadOptions
        {
            HttpHeaders = blobHttpHeader
        });

        // Return full URL to the blob
        return $"{_baseUrl}/{_containerClient.Name}/{fileName}";
    }
}