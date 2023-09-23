using BetterBeatSaber.Colorizer.UI.Config;
using BetterBeatSaber.Core.Provider;
using BetterBeatSaber.Core.Utilities;

using HMUI;

using JetBrains.Annotations;

using UnityEngine;

using Zenject;

namespace BetterBeatSaber.Colorizer.UI.HudModifier; 

public sealed class ComboHudModifier : HudModifier {
    
    #region Injections

    [UsedImplicitly]
    [Inject]
    private readonly ComboController _comboController = null!;
    
    [UsedImplicitly]
    [Inject]
    private readonly ComboUIController _comboUIController = null!;

    [UsedImplicitly]
    [Inject]
    private readonly AssetProvider _assetProvider = null!;
    
    #endregion
    
    private CurvedTextMeshPro? _comboText;
    private CurvedTextMeshPro? _comboNumText;

    private ImageView? _topLine;
    private ImageView? _bottomLine;
    
    private bool _comboBroke;
    
    public void Start() {

        var comboTexts = _comboUIController.GetComponentsInChildren<CurvedTextMeshPro>();
        if (comboTexts is not { Length: 2 })
            return;

        _comboText = comboTexts[0];
        _comboNumText = comboTexts[1];

        if (UIColorizerConfig.Instance.ComboHudModifier.Glow) {
            _comboText.font = _assetProvider.DefaultFontBloom;
            _comboNumText.font = _assetProvider.DefaultFontBloom;
        }
        
        var fullComboLines = _comboUIController.GetComponentsInChildren<ImageView>();

        foreach (var fullComboLine in fullComboLines) {
            fullComboLine.gradient = true;
            if (UIColorizerConfig.Instance.ComboHudModifier.Glow) {
                fullComboLine.material = _assetProvider.DistanceFieldMaterial;
            }
        }

        _topLine = fullComboLines[0];
        _bottomLine = fullComboLines[1];

        _comboController.comboBreakingEventHappenedEvent += OnComboBreakingEventHappenedEvent;
        
    }

    public void Update() {
        
        if(_comboText != null)
            _comboText.color = RGB.Color0;
        
        if (_comboBroke)
            return;
        
        if(_comboNumText != null)
            _comboNumText.color = RGB.Color0;
        
        if (_topLine != null) {
            _topLine.color0 = RGB.Color0;
            _topLine.color1 = RGB.Color0;
        }
        
        // ReSharper disable once InvertIf
        if (_bottomLine != null) {
            _bottomLine.color0 = RGB.Color0;
            _bottomLine.color1 = RGB.Color0;
        }
        
    }
    
    private void OnComboBreakingEventHappenedEvent() {
	    
        _comboBroke = true;
        
        if(_topLine != null) {
            _topLine.gameObject.SetActive(false);
        }
        
        if(_bottomLine != null) {
            _bottomLine.gameObject.SetActive(false);
        }
        
        if(_comboText != null) {
            _comboText.color = Color.green;
        }
        
        _comboController.comboBreakingEventHappenedEvent -= OnComboBreakingEventHappenedEvent;
        
    }

}