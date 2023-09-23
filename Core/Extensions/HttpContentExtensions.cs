using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using ModestTree;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BetterBeatSaber.Core.Extensions; 

public static class HttpContentExtensions {

    internal static readonly JsonSerializerSettings DefaultJsonSerializerSettings = new() {
        NullValueHandling = NullValueHandling.Ignore,
        MissingMemberHandling = MissingMemberHandling.Ignore,
        ContractResolver = new DefaultContractResolver {
            NamingStrategy = new SnakeCaseNamingStrategy()
        }
    };

    internal static CancellationToken DefaultCancellationToken => new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token;
    
    public static async Task<T?> ReadAsJsonAsync<T>(this HttpContent content, JsonSerializerSettings? serializerSettings = null) {
        try {
            var json = await content.ReadAsStringAsync();
            if (json == null || json.IsEmpty())
                return default;
            return JsonConvert.DeserializeObject<T>(json, serializerSettings ?? DefaultJsonSerializerSettings);
        } catch (Exception) {
            Console.WriteLine("Failed to parse Json");
            return default;
        }
    }

}