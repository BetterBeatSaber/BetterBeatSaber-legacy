namespace BetterBeatSaber.Server.Steam; 

public interface ISteamResponse<T> {

    public T Response { get; set; }

}