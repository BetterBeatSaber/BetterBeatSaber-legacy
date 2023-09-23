using System;
using System.Linq;

using BetterBeatSaber.Core.Extensions;

using HMUI;

using UnityEngine;

using Object = UnityEngine.Object;

namespace BetterBeatSaber.Core.UI.Components; 

public sealed class SearchBox {

    private const string PrefabName = "SearchInputField";
    
    public GameObject GameObject { get; private set; }
    
    public InputFieldView InputFieldView { get; private set; }
    
    public CurvedTextMeshPro? PlaceholderText { get; private set; }
    
    public event Action<string>? OnValueChanged;

    private SearchBox(GameObject gameObject) {
        
        GameObject = gameObject;
        
        InputFieldView = gameObject.GetComponent<InputFieldView>();
        InputFieldView.onValueChanged.AddListener(OnInputFieldViewValueChanged);

        PlaceholderText = gameObject.GetComponentsInChildren<CurvedTextMeshPro>()[0];

    }

    private void OnInputFieldViewValueChanged(InputFieldView inputFieldView) {
        OnValueChanged?.Invoke(inputFieldView.text);
    }

    public void SetPlaceholderText(string text) {
        if(PlaceholderText != null)
            PlaceholderText.text = text;
    }

    public void SetKeyboardPositionOffset(Vector3 offset) {
        InputFieldView.SetField("_keyboardPositionOffset", offset);
    }
    
    public static SearchBox? Create(Transform transform) {
        
        var prefabGameObject = Resources.FindObjectsOfTypeAll<InputFieldView>().FirstOrDefault(x => x.gameObject.name == PrefabName)?.gameObject;
        if (prefabGameObject == null)
            return null;
        
        var gameObject = Object.Instantiate(prefabGameObject, transform, false);
        gameObject.name = "SearchBox";
        
        return gameObject != null ? new SearchBox(gameObject) : null;
    }

}