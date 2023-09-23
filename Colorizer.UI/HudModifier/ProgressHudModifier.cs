using BetterBeatSaber.Colorizer.UI.Config;
using BetterBeatSaber.Core.Provider;

using HMUI;

using UnityEngine;

using Zenject;

namespace BetterBeatSaber.Colorizer.UI.HudModifier;

public sealed class ProgressHudModifier : HudModifier {

    #region Injections

    [Inject]
    private readonly SongProgressUIController _songProgressUIController = null!;

    [Inject]
    private readonly AudioTimeSyncController _audioTimeSyncController = null!;

    [Inject]
    private readonly AssetProvider _assetProvider = null!;

    #endregion

    private ImageView? _progressBar;

    public void Start() {

        var progress = _songProgressUIController.transform.Find("Progress");

        if (progress == null)
            return;

        DestroyImmediate(_songProgressUIController.transform.Find("Slider").gameObject);

        _progressBar = progress.gameObject.GetComponent<ImageView>();

        if (UIColorizerConfig.Instance.ProgressHudModifier.Glow) {
            _progressBar.material = _assetProvider.DistanceFieldMaterial;
        }

    }

    public void Update() {
        if (_progressBar != null)
            _progressBar.color = Color.Lerp(Color.red, Color.green, _audioTimeSyncController.songTime / _audioTimeSyncController.songLength);
    }

}