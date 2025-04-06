using System.Reflection;

namespace Domain.Extensions;

public static class MappingExtensions
{
    public static TDestination MapTo<TDestination>(this object source)
    {
        // Kontrollerar att källobjektet inte är null
        ArgumentNullException.ThrowIfNull(source, nameof(source));

        // Skapar en ny instans av destinationstypen
        TDestination destination = Activator.CreateInstance<TDestination>()!;

        // Hämtar alla publika instans-properties från både käll- och destinationsobjekten
        var sourceProperties = source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var destinationProperties = destination.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        // Går igenom alla properties i destinationsobjektet
        foreach (var destinationProperty in destinationProperties)
        {
            // Letar efter matchande property i källobjektet (samma namn och typ)
            var sourceProperty = sourceProperties.FirstOrDefault(x => 
                x.Name == destinationProperty.Name && 
                x.PropertyType == destinationProperty.PropertyType);

            // Om matchande property hittas och kan skrivas till
            if (sourceProperty != null && destinationProperty.CanWrite)
            {
                // Kopierar värdet från käll- till destinationsproperty
                var value = sourceProperty.GetValue(source);
                destinationProperty.SetValue(destination, value);
            }
        }

        return destination;
    }
}