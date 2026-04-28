using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace ASC.Utilities;

public static class SessionExtensions
{
    public static void SetSession(this ISession session, string key, object value)
    {
        session.Set(key, Encoding.ASCII.GetBytes(JsonSerializer.Serialize(value)));
    }

    public static T? GetSession<T>(this ISession session, string key)
    {
        if (session.TryGetValue(key, out byte[]? value))
        {
            return JsonSerializer.Deserialize<T>(Encoding.ASCII.GetString(value));
        }
        return default;
    }
}
