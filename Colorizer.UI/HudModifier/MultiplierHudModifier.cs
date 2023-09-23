using BetterBeatSaber.Colorizer.UI.Config;
using BetterBeatSaber.Core.Extensions;
using BetterBeatSaber.Core.Provider;
using BetterBeatSaber.Core.Utilities;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

namespace BetterBeatSaber.Colorizer.UI.HudModifier;

public sealed class MultiplierHudModifier : HudModifier {

    #region Injections

    [Inject]
    private readonly ScoreMultiplierUIController _scoreMultiplierUIController = null!;

    [Inject]
    private readonly IScoreController _scoreController = null!;

    [Inject]
    private readonly AssetProvider _assetProvider = null!;

    #endregion

    private int _currentMultiplier;

    private Image _background = null!;
    private Image _foreground = null!;

    public void Start() {
        
        _scoreController.multiplierDidChangeEvent += HandleMultiplierDidChange;
        _background = _scoreMultiplierUIController.transform.Find("BGCircle").GetComponent<Image>();
        _foreground = _scoreMultiplierUIController.transform.Find("FGCircle").GetComponent<Image>();

        if (UIColorizerConfig.Instance.MultiplierHudModifier.Glow) {
            _background.material = _assetProvider.DefaultUIMaterial;
            _foreground.material = _assetProvider.DefaultUIMaterial;
        }

        HandleMultiplierDidChange(1, 0f);
        
    }

    public void OnDestroy() {
        _scoreController.multiplierDidChangeEvent -= HandleMultiplierDidChange;
    }

    private void HandleMultiplierDidChange(int multiplier, float progress) {
        _currentMultiplier = multiplier;
    }

    public void Update() {

        if (!_scoreMultiplierUIController.isActiveAndEnabled)
            return;

        switch (_currentMultiplier) {
            case 1:
                _background.color = Color.red.LerpHSV(Color.yellow, _foreground.fillAmount).WithAlpha(.25f);
                _foreground.color = Color.red.LerpHSV(Color.yellow, _foreground.fillAmount);

                break;
            case 2:
                _background.color = Color.yellow.LerpHSV(Color.green, _foreground.fillAmount).WithAlpha(.25f);
                _foreground.color = Color.yellow.LerpHSV(Color.green, _foreground.fillAmount);

                break;
            case 4:
                _background.color = Color.green.LerpHSV(RGB.Color0, _foreground.fillAmount).WithAlpha(.25f);
                _foreground.color = Color.green.LerpHSV(RGB.Color0, _foreground.fillAmount);

                break;
            case 8:
                _background.color = RGB.Color0;

                break;
        }

    }

}