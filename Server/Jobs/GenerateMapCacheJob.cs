using System.IO.Compression;

using BetterBeatSaber.Server.Services.Interfaces;
using BetterBeatSaber.Shared.Serialization;

using Newtonsoft.Json;

namespace BetterBeatSaber.Server.Jobs; 

public sealed class GenerateMapCacheJob : Job<GenerateMapCacheJob> {

    private readonly HttpClient _httpClient;
    
    public GenerateMapCacheJob(IServiceScopeFactory scopeFactory, ILogger<GenerateMapCacheJob> logger, HttpClient httpClient) : base(scopeFactory, logger) {
        _httpClient = httpClient;
    }

    protected override TimeSpan Delay => TimeSpan.Zero;
    protected override TimeSpan Interval => TimeSpan.FromHours(4);
    
    protected override async Task Run() {

        using var scope = ScopeFactory.CreateScope();

        var songService = scope.ServiceProvider.GetService<IMapService>();
        if (songService == null) {
            Logger.LogWarning("Failed to retrieve the SongService");
            return;
        }

        Stream stream;
        try {
            stream = await _httpClient.GetStreamAsync("https://github.com/andruzzzhka/BeatSaberScrappedData/raw/master/combinedScrappedData.zip");
        } catch (Exception exception) {
            Logger.LogWarning("Failed to download the latest Scraped Data: {0}", exception.Message);
            return;
        }
        
        var entry = new ZipArchive(stream).GetEntry("combinedScrappedData.json");
        if (entry == null) {
            Logger.LogWarning("Failed read the Archive");
            return;
        }

        using var streamReader = new StreamReader(entry.Open());

        Song[]? songs;
        try {
            songs = JsonConvert.DeserializeObject<Song[]>(await streamReader.ReadToEndAsync());
        } catch (Exception exception) {
            Logger.LogWarning("Failed to parse the latest Scraped Data: {0}", exception.Message);
            return;
        }
        
        if (songs == null) {
            Logger.LogWarning("Failed to parse the latest Scraped Data");
            return;
        }

        // TODO: Update and make better lmao
        var buffer = new ByteBuffer(ByteBuffer.MaxSize);
        buffer.WriteArray(songs.Select(song => new Shared.Models.Song {
            Key = song.Key,
            Hash = song.Hash,
            SongName = song.SongName,
            SongSubName = song.SongSubName,
            SongAuthorName = song.SongAuthorName,
            LevelAuthorName = song.LevelAuthorName,
            Difficulties = song.Difficulties.Select(difficulty => new Shared.Models.Song.DifficultySong {
                Difficulty = Enum.Parse<Shared.Models.Song.SongDifficulty>(difficulty.Difficulty),
                Characteristic = CharacteristicToEnum(difficulty.Characteristic),
                Stars = difficulty.Stars,
                StarsBl = difficulty.StarsBl,
                Ranked = difficulty.Ranked,
                RankedBl = difficulty.RankedBl,
                RankedUpdateTime = difficulty.RankedUpdateTime,
                Bombs = difficulty.Bombs,
                Notes = difficulty.Notes,
                Obstacles = difficulty.Obstacles,
                Njs = difficulty.Njs,
                NjsOffset = difficulty.NjsOffset,
                Requirements = Shared.Models.Song.SongRequirement.None, // TODO: Update
            }).ToArray(),
            Characteristics = Shared.Models.Song.SongCharacteristic.Standard, // TODO: Update
            Uploaded = song.Uploaded,
            Uploader = song.Uploader,
            Bpm = song.Bpm,
            UpVotes = song.UpVotes,
            DownVotes = song.DownVotes,
            Duration = song.Duration
        }).ToArray());

        songService.MapCacheData = buffer.ToArray();
        
        Logger.LogInformation("Created MapCache");
        
    }

    private Shared.Models.Song.SongCharacteristic CharacteristicToEnum(string characteristic) {
        if (characteristic == "90Degree")
            return Shared.Models.Song.SongCharacteristic.Degree90;
        if (characteristic == "360Degree")
            return Shared.Models.Song.SongCharacteristic.Degree360;
        return Enum.Parse<Shared.Models.Song.SongCharacteristic>(characteristic);
    }

    #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    
    private sealed class Song {

        public string Key { get; set; }
        public string Hash { get; set; }
        public string SongName { get; set; }
        public string SongSubName { get; set; }
        public string SongAuthorName { get; set; }
        public string LevelAuthorName { get; set; }
        [JsonProperty("Diffs")]
        public SongDifficulty[] Difficulties { get; set; }
        [JsonProperty("Chars")]
        public string[] Characteristics { get; set; }
        public DateTime Uploaded { get; set; }
        public string Uploader { get; set; }
        public float Bpm { get; set; }
        public int UpVotes { get; set; }
        public int DownVotes { get; set; }
        public int Duration { get; set; }
        
    }

    private sealed class SongDifficulty {

        [JsonProperty("Diff")]
        public string Difficulty { get; set; }
        [JsonProperty("Char")]
        public string Characteristic { get; set; }
        public float Stars { get; set; }
        public float StarsBl { get; set; }
        public bool Ranked { get; set; }
        public bool RankedBl { get; set; }
        public DateTime RankedUpdateTime { get; set; }
        public int Bombs { get; set; }
        public int Notes { get; set; }
        public int Obstacles { get; set; }
        public float Njs { get; set; }
        public float NjsOffset { get; set; }
        public string[] Requirements { get; set; }

    }
    
}