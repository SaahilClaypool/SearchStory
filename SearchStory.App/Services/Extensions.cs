using System.Text.Json;
using System.Security.Claims;

namespace SearchStory.App
{
    public static class JsonExt
    {
        public static string ToJson<T>(this T self) => JsonSerializer.Serialize(self, new() { ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve  } );
        public static T? FromJson<T>(string inp) => JsonSerializer.Deserialize<T>(inp, new() { PropertyNameCaseInsensitive = true, ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve });
    }
    
    public static class ClaimsExt
    {
    }
}