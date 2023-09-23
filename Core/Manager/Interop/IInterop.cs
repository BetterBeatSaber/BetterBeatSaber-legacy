using BetterBeatSaber.Core.Interfaces;

namespace BetterBeatSaber.Core.Manager.Interop; 

public interface IInterop : IInitializable {

    public bool ShouldRun();

}