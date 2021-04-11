using System.Text.Json;

namespace SearchStory.App
{
    public static class JsonExt
    {
        public static string ToJson<T>(this T self) => JsonSerializer.Serialize(self);
        public static T? FromJson<T>(string inp) => JsonSerializer.Deserialize<T>(inp, new() { PropertyNameCaseInsensitive = true });
    }
}