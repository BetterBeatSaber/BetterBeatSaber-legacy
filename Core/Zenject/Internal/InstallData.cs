using System;

namespace BetterBeatSaber.Core.Zenject.Internal; 

internal sealed class InstallData {

    public Type Type { get; }
    public Location Location { get; }

    public InstallData(Type type, Location location) {
        Type = type;
        Location = location;
    }

}