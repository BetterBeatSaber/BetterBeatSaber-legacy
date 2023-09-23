using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using BetterBeatSaber.Core.Extensions;
using BetterBeatSaber.Core.Utilities;

using Newtonsoft.Json;

namespace BetterBeatSaber.Core.Api;

// ReSharper disable MemberCanBeMadeStatic.Global

public sealed class ApiClient : ConstructableSingleton<ApiClient> {

    public HttpClient HttpClient { get; private set; } = new();
    
    #if STAGING
    private const string APIUrl = "https://staging.betterbs.xyz";
    #elif DEBUG
    private const string APIUrl = "http://localhost:5295";
    #else
    private const string APIUrl = "https://betterbs.xyz";
    #endif
    
    #region Get

    public async Task<HttpResponseMessage?> GetRaw(string path, Dictionary<string, string>? query = null, CancellationToken? cancellationToken = null) =>
        await HttpClient.GetAsync($"{APIUrl}{path}{query.BuildQueryString()}", cancellationToken ?? HttpContentExtensions.DefaultCancellationToken);

    public async Task<T?> Get<T>(string path, Dictionary<string, string>? query = null, CancellationToken? cancellationToken = null) {
        var response = await GetRaw(path, query, cancellationToken);
        if (response is not { IsSuccessStatusCode: true })
            return default;
        return await response.Content.ReadAsJsonAsync<T>();
    }

    #endregion

    #region Post

    public async Task<HttpResponseMessage?> PostRaw(string path, HttpContent? content = null, Dictionary<string, string>? query = null, CancellationToken? cancellationToken = null) =>
        await HttpClient.PostAsync($"{APIUrl}{path}{query.BuildQueryString()}", content, cancellationToken ?? HttpContentExtensions.DefaultCancellationToken);
    
    public async Task<HttpResponseMessage?> Post(string path, object? content = null, Dictionary<string, string>? query = null, CancellationToken? cancellationToken = null) =>
        await PostRaw(path, new StringContent(JsonConvert.SerializeObject(content, HttpContentExtensions.DefaultJsonSerializerSettings), Encoding.UTF8, "application/json"), query, cancellationToken ?? HttpContentExtensions.DefaultCancellationToken);
    
    public async Task<T?> Post<T>(string path, object? body = null, Dictionary<string, string>? query = null, CancellationToken? cancellationToken = null) {
        var response = await Post(path, new StringContent(JsonConvert.SerializeObject(body, HttpContentExtensions.DefaultJsonSerializerSettings)), query, cancellationToken);
        if (response is not { IsSuccessStatusCode: true })
            return default;
        return await response.Content.ReadAsJsonAsync<T>();
    }

    #endregion

    #region Put

    public async Task<HttpResponseMessage?> PutRaw(string path, HttpContent content, Dictionary<string, string>? query = null, CancellationToken? cancellationToken = null) =>
        await HttpClient.PutAsync($"{APIUrl}{path}{query.BuildQueryString()}", content, cancellationToken ?? HttpContentExtensions.DefaultCancellationToken);

    #endregion

    #region Delete

    public async Task<HttpResponseMessage?> DeleteRaw(string path, CancellationToken? cancellationToken = null) =>
        await HttpClient.DeleteAsync($"{APIUrl}{path}", cancellationToken ?? HttpContentExtensions.DefaultCancellationToken);

    #endregion
    
}