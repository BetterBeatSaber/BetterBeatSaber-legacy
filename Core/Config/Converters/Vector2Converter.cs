using System;

using Newtonsoft.Json;

using UnityEngine;

namespace BetterBeatSaber.Core.Config.Converters; 

internal sealed class Vector2Converter : JsonConverter<Vector2> {

    public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer) {
        writer.WriteStartObject();
        writer.WritePropertyName("x");
        writer.WriteValue(value.x);
        writer.WritePropertyName("y");
        writer.WriteValue(value.y);
        writer.WriteEndObject();
    }

    public override Vector2 ReadJson(JsonReader reader, Type objectType, Vector2 existingValue, bool hasExistingValue, JsonSerializer serializer) {
        return JsonConvert.DeserializeObject<Vector2>(serializer.Deserialize(reader)!.ToString());
    }

}