using System.Text;
using System.Text.Json;
using EventStore.Client;

namespace Marketplace.Infrastructure;

public static class EventDeserializer
{
    public static object Deserialize(this ResolvedEvent resolvedEvent)
    {
        var meta = JsonSerializer.Deserialize<EventMetadata>(
            Encoding.UTF8.GetString(resolvedEvent.Event.Metadata.ToArray()));
        var dataType = Type.GetType(meta.ClrType);
        var jsonData = Encoding.UTF8.GetString(resolvedEvent.Event.Data.ToArray());
        var data = JsonSerializer.Deserialize(jsonData, dataType);
        return data;
    }
}
