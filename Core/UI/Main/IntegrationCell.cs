using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;

using BetterBeatSaber.Core.Manager;
using BetterBeatSaber.Shared.Enums;

using JetBrains.Annotations;

using TMPro;

using UnityEngine.UI;

namespace BetterBeatSaber.Core.UI.Main; 

public sealed class IntegrationCell : ListCell<IntegrationType> {

    private IntegrationType _integrationType;
    private bool _isConnected;
    
    [UsedImplicitly]
    [UIComponent(nameof(Name))]
    public readonly TMP_Text Name = null!;

    [UsedImplicitly]
    [UIComponent(nameof(ActionButton))]
    public readonly Button ActionButton = null!;
    
    public override void Populate(IntegrationType integrationType) {

        _integrationType = integrationType;
        _isConnected = IntegrationManager.Instance.GetIntegration(integrationType) != null;
        
        Name.text = integrationType.ToString();
        
        ActionButton.SetButtonText(_isConnected ? "Disconnect" : "Connect");
        
    }

    [UIAction(nameof(OnActionButtonPressed))]
    public void OnActionButtonPressed() {

        ActionButton.interactable = false;

        if (_isConnected) {
            // TODO: Disconnect
        } else {
            // TODO: Connect
        }

    }

}