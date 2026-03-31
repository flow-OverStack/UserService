using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Newtonsoft.Json;

namespace UserService.Api.AuthModels;

internal class RequiredClaims
{
    [JsonProperty(ClaimTypes.NameIdentifier)]
    public long? UserId { get; set; }

    [JsonProperty(JwtRegisteredClaimNames.PreferredUsername)]
    public string? Username { get; set; }

    [JsonProperty(ClaimTypes.Role)]
    [JsonConverter(typeof(SingleOrArrayConverter<string>))]
    public string[]? Roles { get; set; }

    public bool IsValid()
    {
        return UserId != null && Username != null && Roles is { Length: > 0 };
    }

    private sealed class SingleOrArrayConverter<T> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(T[]);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartArray) return serializer.Deserialize<T[]>(reader);

            var item = serializer.Deserialize<T>(reader);
            return !Equals(item, default(T)) ? new[] { item } : Array.Empty<T>();
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}