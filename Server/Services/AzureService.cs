using System.Net;
using System.Net.Http.Headers;

using BetterBeatSaber.Server.Extensions;
using BetterBeatSaber.Server.Services.Interfaces;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BetterBeatSaber.Server.Services;

public sealed class AzureService : IAzureService {

    private const string SubscriptionKeyHeaderName = "Ocp-Apim-Subscription-Key";
    private const string SubscriptionRegionHeaderName = "Ocp-Apim-Subscription-Region";
    
    private const string OutputFormatHeaderName = "X-Microsoft-OutputFormat";

    private readonly ILogger<AzureService> _logger;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    private static readonly Dictionary<int, string> TranslationCache = new();
    private static readonly Dictionary<int, byte[]> AudioCache = new();

    public AzureService(ILogger<AzureService> logger, IConfiguration configuration, HttpClient httpClient) {
        _logger = logger;
        _configuration = configuration;
        _httpClient = httpClient;
    }

    public async Task<string?> Translate(string text, string to) {

        if (TranslationCache.TryGetValue(string.Concat(text, to).GetHashCode(), out var value))
            return value;
        
        var response = await _httpClient.SendAsync(new HttpRequestMessage {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"https://api.cognitive.microsofttranslator.com/translate?api-version=3.0&to={to}"),
            Headers = {
                { SubscriptionKeyHeaderName, _configuration.GetValue<string>("Azure:Translation:SubscriptionKey") },
                { SubscriptionRegionHeaderName, _configuration.GetValue<string>("Azure:Translation:Region") },
                { "User-Agent", "BBS-Server/v2.0.0" }
            },
            Content = new StringContent(JsonConvert.SerializeObject(new TranslationRequest[] {
                new() { Text = text }
            }), new MediaTypeHeaderValue("application/json", "utf-8"))
        });

        if (!response.IsSuccessStatusCode)
            return null;

        var data = await response.Content.ReadAsJsonAsync<JArray>();
        var translation = data?[0]["translations"]?[0]?["text"]?.Value<string>();
        
        if(TranslationCache.Count >= 512)
            TranslationCache.Clear();

        if (translation == null)
            return null;
        
        TranslationCache.Add(string.Concat(text, to).GetHashCode(), translation);
        
        return translation;
        
    }
    
    public async Task<byte[]?> Speak(string ssml) {

        if (AudioCache.TryGetValue(ssml.GetHashCode(), out var value))
            return value;
        
        var response = await _httpClient.SendAsync(new HttpRequestMessage {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"https://{_configuration.GetValue<string>("Azure:Cognitive:Region")}.tts.speech.microsoft.com/cognitiveservices/v1"),
            Headers = {
                { SubscriptionKeyHeaderName, _configuration.GetValue<string>("Azure:Cognitive:SubscriptionKey") },
                { OutputFormatHeaderName, "riff-44100hz-16bit-mono-pcm" },
                { "User-Agent", "BBS-Server/v2.0.0" }
            },
            Content = new StringContent(ssml, new MediaTypeHeaderValue("application/ssml+xml"))
        });

        if (!response.IsSuccessStatusCode) {
            _logger.LogWarning("Failed to Speak: {Error}", response.ReasonPhrase);
            return null;
        }

        var audioData = await response.Content.ReadAsByteArrayAsync();
        
        if(AudioCache.Count >= 128)
            AudioCache.Clear();
        
        AudioCache.Add(ssml.GetHashCode(), audioData);
        
        return audioData;

    }

    public async Task<Dictionary<string, List<string>>> GetVoices() {

        var response = await _httpClient.SendAsync(new HttpRequestMessage {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"https://{_configuration.GetValue<string>("Azure:Cognitive:Region")}.tts.speech.microsoft.com/cognitiveservices/voices/list"),
            Headers = {
                { SubscriptionKeyHeaderName, _configuration.GetValue<string>("Azure:Cognitive:SubscriptionKey") }
            }
        });
        
        var voices = new Dictionary<string, List<string>>();
        if (response.StatusCode != HttpStatusCode.OK)
            return voices;

        foreach (var voice in (await response.Content.ReadAsJsonAsync<List<Voice>>(new JsonSerializerSettings()))!) {
            if (!voices.ContainsKey(voice.Locale))
                voices.Add(voice.Locale, new List<string>());
            voices[voice.Locale].Add(voice.ShortName);
        }

        return voices;

    }

    #pragma warning disable CS8618
    #pragma warning disable CS0649

    // ReSharper disable once ClassNeverInstantiated.Local
    private partial record Voice {

        public string Name;
        public string DisplayName;
        public string LocalName;
        public string ShortName;
        public Gender Gender;
        public string Locale;
        public string LocaleName;
        public string SampleRateHertz;
        public string VoiceType; // Neural, Standard ig?
        public string Status;
        public int WordsPerMinute;

    }

    private enum Gender {

        Male,
        Female

    }

    private partial class TranslationRequest {

        public string Text { get; set; }

    }

}