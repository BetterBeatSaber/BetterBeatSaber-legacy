using TMPro;

using UnityEngine;

namespace BetterBeatSaber.Core.TextMeshPro; 

public sealed class RGBRichTag : MonoBehaviour {

    private TMP_Text _text = null!;

    private void Awake() {
        _text = gameObject.GetComponent<TMP_Text>();
    }

    private void Update() {
        
        _text.ForceMeshUpdate();

        foreach (var linkInfo in _text.textInfo.linkInfo) {
            
            if (linkInfo.GetLinkID() != "rgb")
                continue;
            
            for (var i = linkInfo.linkTextfirstCharacterIndex; i < linkInfo.linkTextfirstCharacterIndex + linkInfo.linkTextLength; i++) {
                
                var characterInfo = _text.textInfo.characterInfo[i];
                if (!characterInfo.isVisible || characterInfo.character == ' ')
                    continue;
                
                var newColors = _text.textInfo.meshInfo[characterInfo.materialReferenceIndex].colors32;
                for (var j = 0; j < 4; j++) {
                    newColors[characterInfo.vertexIndex + j] = Utilities.RGB.Color0;
                }
                
            }
            
        }
        
        _text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
        
    }

}