namespace BetterBeatSaber.Server.Interfaces; 

public interface ISharedConvertable<out T> {

    public T ToSharedModel();

}