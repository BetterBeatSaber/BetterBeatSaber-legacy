using BetterBeatSaber.Core.Extensions;
using BetterBeatSaber.Core.Utilities;

using IPA.Utilities;

using UnityEngine;

namespace BetterBeatSaber.Colorizer.Game.Colorizer; 

public sealed class ObstacleColorizer : MonoBehaviour {

    private static readonly FieldAccessor<StretchableObstacle, ParametricBoxFrameController>.Accessor ObstacleFrameAccessor = FieldAccessor<StretchableObstacle, ParametricBoxFrameController>.GetAccessor("_obstacleFrame");
    private static readonly FieldAccessor<StretchableObstacle, ParametricBoxFakeGlowController>.Accessor ObstacleFakeGlowAccessor = FieldAccessor<StretchableObstacle, ParametricBoxFakeGlowController>.GetAccessor("_obstacleFakeGlow");
    private static readonly FieldAccessor<StretchableObstacle, float>.Accessor AddColorMultiplierAccessor = FieldAccessor<StretchableObstacle, float>.GetAccessor("_addColorMultiplier");
    private static readonly FieldAccessor<StretchableObstacle, float>.Accessor ObstacleCoreLerpToWhiteFactorAccessor = FieldAccessor<StretchableObstacle, float>.GetAccessor("_obstacleCoreLerpToWhiteFactor");
    private static readonly FieldAccessor<StretchableObstacle, MaterialPropertyBlockController[]>.Accessor MaterialPropertyBlockControllersAccessor = FieldAccessor<StretchableObstacle, MaterialPropertyBlockController[]>.GetAccessor("_materialPropertyBlockControllers");

    private static readonly int TintColorID = Shader.PropertyToID("_TintColor");
    private static readonly int AddColorID = Shader.PropertyToID("_AddColor");

    private ParametricBoxFrameController _obstacleFrame = null!;
    private ParametricBoxFakeGlowController _obstacleFakeGlow = null!;
    private float _addColorMultiplier;
    private float _obstacleCoreLerpToWhiteFactor;
    private MaterialPropertyBlockController[] _materialPropertyBlockControllers = null!;

    private void Start() {

        var stretchableObstacle = gameObject.GetComponent<ObstacleController>().GetComponent<StretchableObstacle>();
        
        _obstacleFrame = ObstacleFrameAccessor(ref stretchableObstacle);
        _obstacleFakeGlow = ObstacleFakeGlowAccessor(ref stretchableObstacle);
        _addColorMultiplier = AddColorMultiplierAccessor(ref stretchableObstacle);
        _obstacleCoreLerpToWhiteFactor = ObstacleCoreLerpToWhiteFactorAccessor(ref stretchableObstacle);
        _materialPropertyBlockControllers = MaterialPropertyBlockControllersAccessor(ref stretchableObstacle);

    }

    private void Update() {
        
        var color = RGB.Color0;

        _obstacleFrame.color = color;
        _obstacleFrame.Refresh();
        if (_obstacleFakeGlow != null) {
            _obstacleFakeGlow.color = color;
            _obstacleFakeGlow.Refresh();
        }

        var value = (color * _addColorMultiplier).WithAlpha(0f);
        
        foreach (var materialPropertyBlockController in _materialPropertyBlockControllers) {
            materialPropertyBlockController.materialPropertyBlock.SetColor(AddColorID, value);
            materialPropertyBlockController.materialPropertyBlock.SetColor(TintColorID, Color.Lerp(color, Color.white, _obstacleCoreLerpToWhiteFactor));
            materialPropertyBlockController.ApplyChanges();
        }
        
    }

}