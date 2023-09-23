using TMPro;

using UnityEngine;

namespace BetterBeatSaber.Core.TextMeshPro; 

public sealed class RGBGradientRichTag : MonoBehaviour {

    private TMP_Text _text = null!;

    private void Awake() {
        _text = gameObject.GetComponent<TMP_Text>();
    }
    
    private void Update() {
        
        _text.ForceMeshUpdate();

        foreach (var linkInfo in _text.textInfo.linkInfo) {

            if (linkInfo.GetLinkID() != "rgb-gradient")
                continue;

            Color[] Steps(int stepsAmount) {

                stepsAmount += 2;
                
                var t = Time.time / 1f;
                var hue = Mathf.Clamp(t % 2f >= 1f ? 1f - t % 1f : t % 1f, .05f, .9f);
                
                var start = Color.HSVToRGB(hue, 1f, 1f);
                var end = Color.HSVToRGB(Mathf.Max(1f, hue + .15f), 1f, 1f);
                
                var result = new Color[stepsAmount];
                var r = (end.r - start.r) / (stepsAmount - 1);
                var g = (end.g - start.g) / (stepsAmount - 1);
                var b = (end.b - start.b) / (stepsAmount - 1);
                var a = (end.a - start.a) / (stepsAmount - 1);
                
                for (var index = 0; index < stepsAmount; index++) {
                    result[index] = new Color(start.r + r * index, start.g + g * index, start.b + b * index, start.a + a * index);
                }

                return result;
            }
            
            var linkTextLength = linkInfo.linkTextLength;
            var linkTextFirstCharacterIndex = linkInfo.linkTextfirstCharacterIndex;
            
            var steps = Steps(linkTextLength);
            var gradients = new VertexGradient[linkTextLength];
            for (var index = linkTextFirstCharacterIndex; index < linkTextFirstCharacterIndex + linkTextLength; index++) {

                var i = index - linkTextFirstCharacterIndex;
                gradients[i] = new VertexGradient(steps[i], steps[i + 1], steps[i], steps[i + 1]);
                
                var characterInfo = _text.textInfo.characterInfo[index];
                if (!characterInfo.isVisible || characterInfo.character == ' ')
                    continue;
                
                var colors = _text.textInfo.meshInfo[characterInfo.materialReferenceIndex].colors32;
                
                var vertexIndex = _text.textInfo.characterInfo[index].vertexIndex;
                colors[vertexIndex + 0] = gradients[index].bottomLeft;
                colors[vertexIndex + 1] = gradients[index].topLeft;
                colors[vertexIndex + 2] = gradients[index].bottomRight;
                colors[vertexIndex + 3] = gradients[index].topRight;
                
            }
            
        }
        
        _text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);

    }

}