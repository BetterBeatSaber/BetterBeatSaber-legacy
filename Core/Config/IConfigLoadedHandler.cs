namespace BetterBeatSaber.Core.Config; 

public interface IConfigLoadedHandler {

    public void OnLoaded(bool fromCloud);

}