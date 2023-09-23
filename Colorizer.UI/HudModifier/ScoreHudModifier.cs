using System.Collections.Generic;

using BetterBeatSaber.Colorizer.UI.Config;
using BetterBeatSaber.Core.Provider;
using BetterBeatSaber.Core.Utilities;

using IPA.Utilities;

using TMPro;

using UnityEngine;

using Zenject;

namespace BetterBeatSaber.Colorizer.UI.HudModifier; 

#pragma warning disable CS8618

public sealed class ScoreHudModifier : HudModifier {

    #region Injections

    [Inject]
    private readonly RelativeScoreAndImmediateRankCounter _relativeScoreAndImmediateRankCounter = null!;

    [Inject]
    private readonly ImmediateRankUIPanel _immediateRankUIPanel = null!;

    [Inject]
    private readonly AssetProvider _assetProvider = null!;
    
    #endregion

    private TextMeshProUGUI _rankText = null!;
    private TextMeshProUGUI _scoreText = null!;

    private bool _rgb = true;

    private TMP_FontAsset _rankTextDefaultFont;
    private TMP_FontAsset _scoreTextDefaultFont;

    public void Start() {
        
        _rankText = _immediateRankUIPanel.GetField<TextMeshProUGUI, ImmediateRankUIPanel>("_rankText");
        _scoreText = _immediateRankUIPanel.GetField<TextMeshProUGUI, ImmediateRankUIPanel>("_relativeScoreText");

        _rankTextDefaultFont = _rankText.font;
        _scoreTextDefaultFont = _scoreText.font;

        _relativeScoreAndImmediateRankCounter.relativeScoreOrImmediateRankDidChangeEvent += OnRelativeScoreOrImmediateRankDidChangeEvent;

        OnRelativeScoreOrImmediateRankDidChangeEvent();

    }

    public void OnDestroy() {
        _relativeScoreAndImmediateRankCounter.relativeScoreOrImmediateRankDidChangeEvent -= OnRelativeScoreOrImmediateRankDidChangeEvent;
    }

    private void OnRelativeScoreOrImmediateRankDidChangeEvent() {
        
        var immediateRank = _relativeScoreAndImmediateRankCounter.immediateRank;

        if (!UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks.ContainsKey(immediateRank) || !UIColorizerConfig.Instance.ScoreHudModifier.CustomRanks.TryGetValue(immediateRank, out var config) || !config.Enabled)
            return;

        _rankText.font = config.Glow ? _assetProvider.DefaultFontBloom : _rankTextDefaultFont;
        _scoreText.font = config.Glow ? _assetProvider.DefaultFontBloom : _scoreTextDefaultFont;
        
        _rgb = config.RGB;
        
        if (_rgb)
            return;
        
        _rankText.color = config.Color;
        _scoreText.color = config.Color;

    }

    public void Update() {
        
        if (!_rgb)
            return;
        
        _rankText.color = RGB.Color0;
        _scoreText.color = RGB.Color0;
        
    }

    public partial class Options : BaseOptions {

        public Dictionary<RankModel.Rank, RankConfig> CustomRanks { get; set; } = new() {
            {
                RankModel.Rank.SS, new RankConfig {
                    Enabled = true,
                    Name = "SUS",
                    Color = new Color(0f, .5f, .5f),
                    RGB = true,
                    Glow = true
                }
            }, {
                RankModel.Rank.S, new RankConfig {
                    Enabled = true,
                    Name = "OK",
                    Color = new Color(1f, 1f, 1f),
                    Glow = true
                }
            }, {
                RankModel.Rank.A, new RankConfig {
                    Enabled = true,
                    Name = "MHH",
                    Color = new Color(0f, 1f, 0f),
                    Glow = true
                }
            }, {
                RankModel.Rank.B, new RankConfig {
                    Enabled = true,
                    Name = "DEAD",
                    Color = new Color(.5f, .5f, 0f),
                    Glow = true
                }
            }, {
                RankModel.Rank.C, new RankConfig {
                    Enabled = true,
                    Name = "DEAD",
                    Color = new Color(1f, .5f, 0f),
                    Glow = true
                }
            }, {
                RankModel.Rank.D, new RankConfig {
                    Enabled = true,
                    Name = "DEAD",
                    Color = new Color(1f, .3f, 0f),
                    Glow = true
                }
            }, {
                RankModel.Rank.E, new RankConfig {
                    Enabled = true,
                    Name = "DEAD",
                    Color = new Color(1f, 0f, 0f),
                    Glow = true
                }
            }
        };

        public partial class RankConfig {

            public bool Enabled { get; set; } = true;
            public string Name { get; set; }
            public Color Color { get; set; }
            public bool RGB { get; set; }
            public bool Glow { get; set; } = true;

        }

    }

}