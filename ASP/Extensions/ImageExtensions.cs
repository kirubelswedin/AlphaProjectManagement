
namespace ASP.Extensions;


// - If the image is stored in Azure (starts with http/https), it returns the cloud URL directly.
// - If the image is local or missing, it builds a local path or a default image path.
// This ensures the frontend always gets a valid image URL, regardless of storage type.
public static class ImageExtensions
{
    public static string GetImageUrl(this string? imageUrl, string type)
    {
        // If image is missing or is a default, return the default image path
        if (string.IsNullOrWhiteSpace(imageUrl) || imageUrl.StartsWith("default-"))
            return $"/images/default-images/{imageUrl ?? $"default-{type.TrimEnd('s')}.svg"}";

        // If image is stored in Azure or already a valid URL/path, return as is
        if (imageUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            imageUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
            imageUrl.StartsWith("/images/", StringComparison.OrdinalIgnoreCase))
            return imageUrl;

        // If image path starts with /{type}/, prepend /images
        if (imageUrl.StartsWith($"/{type}/", StringComparison.OrdinalIgnoreCase))
            return $"/images{imageUrl}";

        // If image path starts with {type}/, prepend /images/
        if (imageUrl.StartsWith($"{type}/", StringComparison.OrdinalIgnoreCase))
            return $"/images/{imageUrl}";

        // If imageUrl does not contain a slash, assume it's just a filename and build path
        if (!imageUrl.Contains('/'))
            return $"/images/{type}/{imageUrl}";

        // Otherwise, trim any leading slash and build the path
        imageUrl = imageUrl.TrimStart('/');
        return $"/images/{type}/{imageUrl}";
    }
}
