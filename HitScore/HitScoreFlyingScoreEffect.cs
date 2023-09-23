using BetterBeatSaber.Core.Extensions;
using BetterBeatSaber.Core.Provider;
using BetterBeatSaber.Core.TextMeshPro;
using BetterBeatSaber.HitScore.Config;

using UnityEngine;

namespace BetterBeatSaber.HitScore; 

public sealed class HitScoreFlyingScoreEffect : FlyingScoreEffect {

    private static readonly Color Color112 = new(.3f, 0f, 1f);
    private static readonly Color Color110 = new(0f, 1f, 1f);
    private static readonly Color Color107 = new(0f, 1f, 0f);
    private static readonly Color Color105 = new(1f, .5f, 0f);
    private static readonly Color Color0 = new(1f, 0f, 0f);

    private NoteCutInfo? _noteCutInfo;

    public override void InitAndPresent(IReadonlyCutScoreBuffer cutScoreBuffer, float duration, Vector3 targetPos, Color color) {
        
        _noteCutInfo = cutScoreBuffer.noteCutInfo;

        _color = color;
        _cutScoreBuffer = cutScoreBuffer;
        if (!cutScoreBuffer.isFinished) {
            cutScoreBuffer.RegisterDidChangeReceiver(this);
            cutScoreBuffer.RegisterDidFinishReceiver(this);
            _registeredToCallbacks = true;
        }

        if (_noteCutInfo!.Value.noteData.gameplayType is not NoteData.GameplayType.Normal) {
            _text.text = cutScoreBuffer.cutScore.ToString();
            _maxCutDistanceScoreIndicator.enabled = cutScoreBuffer.centerDistanceCutScore == cutScoreBuffer.noteScoreDefinition.maxCenterDistanceCutScore;
            _colorAMultiplier = cutScoreBuffer.cutScore > (double)cutScoreBuffer.maxPossibleCutScore * 0.9f ? 1f : 0.3f;
        } else {
            _maxCutDistanceScoreIndicator.enabled = false;

            // Apply judgments a total of twice - once when the effect is created, once when it finishes.
            Judge((CutScoreBuffer) cutScoreBuffer, 30);
        }

        InitAndPresent(duration, targetPos, cutScoreBuffer.noteCutInfo.worldRotation, false);
    }

    protected override void ManualUpdate(float t) {
        var color = _color.WithAlpha(_fadeAnimationCurve.Evaluate(t));
        _text.color = color;
        _maxCutDistanceScoreIndicator.color = color;
    }

    public override void HandleCutScoreBufferDidChange(CutScoreBuffer cutScoreBuffer) {
        
        if (_noteCutInfo!.Value.noteData.gameplayType is not NoteData.GameplayType.Normal) {
            base.HandleCutScoreBufferDidChange(cutScoreBuffer);
            return;
        }

        Judge(cutScoreBuffer);
        
    }

    public override void HandleCutScoreBufferDidFinish(CutScoreBuffer cutScoreBuffer) {
        if (_noteCutInfo!.Value.noteData.gameplayType is NoteData.GameplayType.Normal) {
            Judge(cutScoreBuffer);
        }

        base.HandleCutScoreBufferDidFinish(cutScoreBuffer);
    }

    private void Judge(CutScoreBuffer cutScoreBuffer, int? assumedAfterCutScore = null) {

        TextMeshProAddon.RemoveAny(_text);

        var before = cutScoreBuffer.beforeCutScore;
        var after = assumedAfterCutScore ?? cutScoreBuffer.afterCutScore;
        var accuracy = cutScoreBuffer.centerDistanceCutScore;
        
        var total = before + after + accuracy;

        Color? color = null;
        var size = 100;
        switch (total) {
            case >= 115:
                size = 250;
                break;
            case >= 114:
                size = 225;
                break;
            case >= 112:
                size = 200;
                color = Color112;
                break;
            case >= 110:
                size = 175;
                color = Color110;
                break;
            case >= 107:
                size = 162;
                color = Color107;
                break;
            case >= 105:
                size = 150;
                color = Color105;
                break;
            default:
                size = 125;
                color = Color0;
                break;
        }

        size = (int) (size * HitScoreConfig.Instance.Scale);

        if(HitScoreConfig.Instance.EnableGlow)
            _text.font = AssetProvider.Instance?.DefaultFontBloom;
        
        _text.richText = true;
        _text.text = $"<{(color.HasValue ? $"color=#{color.Value.ToHex()}" : "rgb-gradient")}><size={size}%>{(before < 70 ? "<" : "")}{accuracy}{(after < 30 ? ">" : "")}</size></{(color.HasValue ? "color" : "rgb-gradient")}>";
        
        if(!color.HasValue)
            TextMeshProAddon.Apply(_text);
        
    }

}