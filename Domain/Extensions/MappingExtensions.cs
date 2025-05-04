using System.Reflection;

namespace Domain.Extensions;

public static class MappingExtensions
{
    // maps all matching public properties from source object to new instance of destination type.
    // helps copy values between objects of different types that share property names and types, without manually mapping each property.
    public static TDestination MapTo<TDestination>(this object source)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        TDestination destination = Activator.CreateInstance<TDestination>()!;
        var sourceProperties = source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var destinationProperties = destination.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var destinationProperty in destinationProperties)
        {
            var sourceProperty = sourceProperties.FirstOrDefault(x => 
                x.Name == destinationProperty.Name && 
                x.PropertyType == destinationProperty.PropertyType);

            if (sourceProperty != null && destinationProperty.CanWrite)
            {
                var value = sourceProperty.GetValue(source);
                destinationProperty.SetValue(destination, value);
            }
        }

        return destination;
    }
}