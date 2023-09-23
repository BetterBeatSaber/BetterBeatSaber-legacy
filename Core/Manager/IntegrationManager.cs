using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BetterBeatSaber.Core.Api;
using BetterBeatSaber.Core.Network;
using BetterBeatSaber.Shared.Enums;
using BetterBeatSaber.Shared.Models;
using BetterBeatSaber.Shared.Network.Packets;

namespace BetterBeatSaber.Core.Manager; 

public sealed class IntegrationManager : Manager<IntegrationManager> {

    public List<Integration> Integrations { get; private set; } = new();

    public event Action<IntegrationType, Integration?>? OnIntegrationUpdated;

    public override void Init() {
        NetworkClient.Instance.RegisterPacketHandler<IntegrationUpdatedPacket>(OnIntegrationUpdatedPacketReceived);
        AsyncHelper.RunSync(FetchIntegrations);
    }

    public override void Exit() {
        NetworkClient.Instance.UnregisterPacketHandler<IntegrationUpdatedPacket>();
    }

    private void OnIntegrationUpdatedPacketReceived(IntegrationUpdatedPacket packet) {
        Integrations.RemoveAll(integration => integration.Type == packet.Type);
        Integrations.Add(packet.Integration);
        OnIntegrationUpdated?.Invoke(packet.Type, packet.Integration);
    }
    
    private async Task FetchIntegrations() {
        Integrations = await ApiClient.Instance.Get<List<Integration>>("/integrations") ?? Enumerable.Empty<Integration>().ToList();
    }

    public Integration? GetIntegration(IntegrationType type) {
        return Integrations.FirstOrDefault(integration => integration.Type == type);
    }

    public async Task RemoveIntegration(IntegrationType type) {
        
    }

    public async Task Connect(IntegrationType type) {
        
    }

}