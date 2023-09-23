using System;

namespace BetterBeatSaber.Core.Zenject.Internal; 

internal sealed class ExposeData {

    public Type Type { get; }
    public string ContractName { get; }

    public ExposeData(Type type, string contractName) {
        Type = type;
        ContractName = contractName;
    }

}