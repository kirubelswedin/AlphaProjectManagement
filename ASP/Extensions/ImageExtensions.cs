namespace ASP.Extensions;


// - If the image is stored in Azure (starts with http/https), it returns the cloud URL directly.
// - If the image is local or missing, it builds a local path or a default image path.
// This ensures the frontend always gets a valid image URL, regardless of storage type.
public static class ImageExtensions
{
    // took some help from chatGPT to get this to work as I wanted  
    public static string GetImageUrl(this string? imageUrl, string type)
    {
        if (string.IsNullOrWhiteSpace(imageUrl) || imageUrl.StartsWith("default-"))
            return $"/images/default-images/{imageUrl ?? $"default-{type.TrimEnd('s')}.svg"}";
        
        if (imageUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            imageUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
            imageUrl.StartsWith("/images/", StringComparison.OrdinalIgnoreCase))
            return imageUrl;
        
        if (imageUrl.StartsWith($"/{type}/", StringComparison.OrdinalIgnoreCase))
            return $"/images{imageUrl}";
        
        if (imageUrl.StartsWith($"{type}/", StringComparison.OrdinalIgnoreCase))
            return $"/images/{imageUrl}";
        
        if (!imageUrl.Contains('/'))
            return $"/images/{type}/{imageUrl}";

        // Otherwise, trim any leading slash and build the path
        imageUrl = imageUrl.TrimStart('/');
        return $"/images/{type}/{imageUrl}";
    }
}
