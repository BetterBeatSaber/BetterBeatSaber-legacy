using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BetterBeatSaber.Server.Extensions; 

public static class HttpExtensions {

    internal static readonly JsonSerializerSettings DefaultJsonSerializerSettings = new() {
        NullValueHandling = NullValueHandling.Ignore,
        MissingMemberHandling = MissingMemberHandling.Ignore,
        Error = (_, args) => { args.ErrorContext.Handled = true; },
        ContractResolver = new DefaultContractResolver {
            NamingStrategy = new SnakeCaseNamingStrategy()
        }
    };

    #region Client

    public static async Task<T?> GetJsonAsync<T>(this HttpClient httpClient, string url, JsonSerializerSettings? serializerSettings = null) =>
        JsonConvert.DeserializeObject<T>(await httpClient.GetStringAsync(url), serializerSettings ?? DefaultJsonSerializerSettings);

    #endregion

    #region Content

    public static async Task<T?> ReadAsJsonAsync<T>(this HttpContent content, JsonSerializerSettings? serializerSettings = null) =>
        JsonConvert.DeserializeObject<T>(await content.ReadAsStringAsync(), serializerSettings ?? DefaultJsonSerializerSettings);

    #endregion
    
    #region Context

    public static string GetIP(this HttpContext httpContext) =>
        (string?) httpContext.Request.Headers["Cf-Connecting-Ip"] ?? "127.0.0.1";

    public static string GetCountry(this HttpContext httpContext) =>
        (string?) httpContext.Request.Headers["Cf-Country"] ?? "GL";

    #endregion

}