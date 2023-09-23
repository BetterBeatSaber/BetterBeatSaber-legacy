namespace BetterBeatSaber.Server.Services.Interfaces; 

public interface IAzureService {

    public Task<string?> Translate(string text, string to);
    public Task<byte[]?> Speak(string ssml);
    public Task<Dictionary<string, List<string>>> GetVoices();

}