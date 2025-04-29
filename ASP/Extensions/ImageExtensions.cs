namespace ASP.Extensions;

public static class ImageExtensions
{
    public static string GetImageUrl(this string? imageUrl, string type)
    {
        // always point to the default-images folder
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

        imageUrl = imageUrl.TrimStart('/');
        return $"/images/{type}/{imageUrl}";
    }
}
