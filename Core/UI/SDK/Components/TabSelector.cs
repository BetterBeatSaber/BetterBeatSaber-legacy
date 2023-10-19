using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using BetterBeatSaber.Core.Extensions;
using BetterBeatSaber.Core.UI.SDK.Attributes;

using HMUI;

using UnityEngine;

using Zenject;

using Object = UnityEngine.Object;

namespace BetterBeatSaber.Core.UI.SDK.Components; 

public sealed class TabSelector : Component {

    private static TextSegmentedControl? _segmentControlTemplate;

    private TextSegmentedControl? _textSegmentedControl;
    private TabSelectorBehaviour? _behaviour;
    
    [IgnoreProperty]
    public List<Tab> Tabs { get; } = new();
    
    private readonly List<Tab> tabs = new();

    private int _pageCount = -1;
    private int _currentPage;

    public override GameObject Create(Transform parent, XmlNode node) {
        
        if (_segmentControlTemplate == null)
            _segmentControlTemplate = Resources.FindObjectsOfTypeAll<TextSegmentedControl>().FirstOrDefault(textSegmentedControl => textSegmentedControl.transform.parent.name == "PlayerStatisticsViewController" && textSegmentedControl.GetField<TextSegmentedControl, DiContainer>("_container") != null);

        _textSegmentedControl = BeatSaberUI.Instance.DiContainer?.InstantiatePrefabForComponent<TextSegmentedControl>(_segmentControlTemplate, parent);
        if (_textSegmentedControl == null)
            throw new Exception("Failed to create");

        GameObject = _textSegmentedControl.gameObject;
        
        _textSegmentedControl.name = nameof(TabSelector);
        
        (_textSegmentedControl.transform as RectTransform)!.anchoredPosition = new Vector2(0, 0);
        
        foreach (Transform transform in _textSegmentedControl.transform)
            Object.Destroy(transform.gameObject);

        _behaviour = GameObject.AddComponent<TabSelectorBehaviour>();
        
        return GameObject;
        
    }

    public override void PostCreation(ParseContext context) {

        Tabs.Clear();
        
        foreach (var tab in context.GetComponentsByType<Tab>()) {
            Tabs.Add(tab);
            tab.Selector = this;
        }
        
        Console.WriteLine("ADDED " + Tabs.Count);
        
        // Maybe add to left and to right buttons lols
        https://github.com/monkeymanboy/BeatSaberMarkupLanguage/blob/master/BeatSaberMarkupLanguage/Components/TabSelector.cs#L41

        Refresh();

        if (_textSegmentedControl == null)
            return;
        
        _textSegmentedControl.didSelectCellEvent -= TabSelected;
        _textSegmentedControl.didSelectCellEvent += TabSelected;
        
        _textSegmentedControl.SelectCellWithNumber(0);
        
        TabSelected(_textSegmentedControl, 0);

    }

    public void Refresh() {
        
        var visibleTabs = tabs.Where(tab => tab.Visible).ToList();
        if (PageCount == -1) {
            SetSegmentedControlTexts(visibleTabs);
        } else {
            
            if (_currentPage < 0)
                _currentPage = 0;

            if (_currentPage > (visibleTabs.Count - 1) / _pageCount)
                _currentPage = (visibleTabs.Count - 1) / _pageCount;

            SetSegmentedControlTexts(visibleTabs.Skip(PageCount * _currentPage).Take(PageCount).ToList());
            
            // Maybe add to left and to right buttons lols
            // https://github.com/monkeymanboy/BeatSaberMarkupLanguage/blob/f6eceeabedcc83a762c7a7a66ddb26b63166bc74/BeatSaberMarkupLanguage/Components/TabSelector.cs#L105C1-L105C1

            TabSelected(null, 0);
            
        }
    }
    
    public int PageCount
    {
        get => _pageCount;
        set
        {
            _pageCount = value;
            if (tabs.Count > 0)
            {
                Refresh();
            }
        }
    }

    private void TabSelected(SegmentedControl? segmentedControl, int index) {
       
        if (PageCount != -1)
            index += PageCount * _currentPage;

        foreach (var tab in tabs)
            // ReSharper disable once Unity.NoNullPropagation
            tab.GameObject?.SetActive(false);

        if (index >= tabs.Count(tab => tab.Visible))
            return;

        // ReSharper disable once Unity.NoNullPropagation
        tabs.Where(tab => tab.Visible).ElementAt(index).GameObject?.SetActive(true);
        
    }

    private void SetSegmentedControlTexts(List<Tab> tabs) {
        // ReSharper disable once Unity.NoNullPropagation
        _textSegmentedControl?.SetTexts(tabs.Select(tab => tab.Name).ToArray());
    }

    private void PageLeft() {
        _currentPage--;
        Refresh();
    }

    private void PageRight() {
        _currentPage++;
        Refresh();
    }

    private sealed class TabSelectorBehaviour : MonoBehaviour {

        public TabSelector? TabSelector { get; private set; }

        private bool _shouldRefresh;
        
        private void OnEnable() {
            if (_shouldRefresh)
                Refresh();
        }

        private void Refresh() {
            
            if (!isActiveAndEnabled) {
                _shouldRefresh = true;
                return;
            }

            _shouldRefresh = false;

            TabSelector?.Refresh();
            
        }
        
    }

}