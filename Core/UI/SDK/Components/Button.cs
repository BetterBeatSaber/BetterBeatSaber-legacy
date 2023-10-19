using System;
using System.Linq;
using System.Xml;

using BeatSaberMarkupLanguage.Components;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

using Object = UnityEngine.Object;

namespace BetterBeatSaber.Core.UI.SDK.Components; 

public class Button : Component {

    private static UnityEngine.UI.Button? _buttonPrefab;
    private static UnityEngine.UI.Button? _buttonPrefabPrimary;

    private UnityEngine.UI.Button? _button;
    private TextMeshProUGUI? _text;
    
    public bool Primary { get; init; }

    public event Action? OnClicked; 

    private bool _interactable = true;
    public bool Interactable {
        // ReSharper disable once Unity.NoNullPropagation
        get => _button?.interactable ?? _interactable;
        set {
            _interactable = value;
            if (_button != null)
                _button.interactable = value;
        }
    }

    private bool _rich = true;
    public bool Rich {
        // ReSharper disable once Unity.NoNullPropagation
        get => _text?.richText ?? _rich;
        set {
            _rich = value;
            if (_text != null)
                _text.richText = value;
        }
    }
    
    public override GameObject Create(Transform parent, XmlNode node) {
        
        _buttonPrefab ??= Resources.FindObjectsOfTypeAll<UnityEngine.UI.Button>().Last(x => x.name == "PracticeButton");
        _buttonPrefabPrimary ??= Resources.FindObjectsOfTypeAll<UnityEngine.UI.Button>().Last(x => x.name == "PlayButton");

        _button = Object.Instantiate(Primary ? _buttonPrefabPrimary : _buttonPrefab, parent, false);

        _button.name = nameof(Button);
        _button.interactable = _interactable;

        GameObject = _button.gameObject;
        GameObject.SetActive(true);

        var externalComponents = GameObject.AddComponent<ExternalComponents>();
        var textObject = _button.transform.Find("Content/Text").gameObject;

        _text = textObject.GetComponent<TextMeshProUGUI>();
        
        _text.text = nameof(Button);
        _text.richText = _rich;
        
        externalComponents.components.Add(_text);

        Object.Destroy(_button.transform.Find("Content").GetComponent<LayoutElement>());

        var buttonSizeFitter = GameObject.AddComponent<ContentSizeFitter>();
        
        buttonSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        buttonSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

        var stackLayoutGroup = _button.GetComponentInChildren<LayoutGroup>();
        if (stackLayoutGroup != null)
            externalComponents.components.Add(stackLayoutGroup);
        
        return GameObject;
        
    }

}