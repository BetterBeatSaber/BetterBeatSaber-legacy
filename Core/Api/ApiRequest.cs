using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using BetterBeatSaber.Core.Extensions;
using BetterBeatSaber.Core.Manager;

using Newtonsoft.Json;

using UnityEngine.Networking;

namespace BetterBeatSaber.Core.Api; 

// ReSharper disable once InconsistentNaming
public class ApiRequest<T, B> {

    public UnityWebRequest Request { get; }
    
    public B? Body { get; set; }
    public byte[]? BodyRaw { get; set; }
    public string? ContentType { get; set; }

    public T? Response { get; private set; }
    public bool ResponseRaw { get; set; }

    public string Error { get; private set; } = string.Empty;

    public bool Failed => Error != string.Empty;

    // ReSharper disable once MemberCanBeProtected.Global
    public ApiRequest(string path, string method = "GET", Dictionary<string, string>? query = null) {
        Request = new UnityWebRequest(ApiClient.BuildUrl(path, query), method);
    }

    public IEnumerator Send() {

        Request.downloadHandler = new DownloadHandlerBuffer();
        
        Request.SetRequestHeader("User-Agent", $"BetterBeatSaber/{BetterBeatSaber.Version}");
        
        if(AuthManager.Instance.IsAuthenticated)
            Request.SetRequestHeader("Authorization", $"Bearer {AuthManager.Instance.Token}");
        
        if (Body != null) {
            Request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(Body, HttpContentExtensions.DefaultJsonSerializerSettings)));
            Request.SetRequestHeader("Content-Type", "application/json; charset=utf-8");
        } else if (BodyRaw != null) {
            Request.uploadHandler = new UploadHandlerRaw(BodyRaw);
            Request.SetRequestHeader("Content-Type", ContentType ?? "text/plain");
        }
        
        yield return Request.SendWebRequest();

        if (Request.isHttpError || Request.isNetworkError) {
            Error = Request.error;
            yield break;
        }

        if (ResponseRaw) {
            if (typeof(T) == typeof(string)) {
                Response = (T)(object) Request.downloadHandler.text;
            } else if (typeof(T) == typeof(byte[])) {
                Response = (T)(object) Request.downloadHandler.data;
            } else Error = "The Response type can only be a string or an byte array";
            yield break;
        }
        
        try {
            Response = JsonConvert.DeserializeObject<T>(Request.downloadHandler.text, HttpContentExtensions.DefaultJsonSerializerSettings);
        } catch (Exception exception) {
            Error = exception.Message;
        }
        
    }

}

public class ApiRequest<T> : ApiRequest<T, object> {

    public ApiRequest(string path, string method = "GET", Dictionary<string, string>? query = null) : base(path, method, query) { }

}