using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Diagnostics;

namespace MarketStore.constants
{
    public static class SessionHelper
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value,
                Formatting.None, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                }));
        }
        public static void RemoveSession(this ISession session, string key)
        {
            session.Remove(key);
        }

        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
