using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using BetterBeatSaber.Core.Extensions;

using HMUI;

using UnityEngine;

using Zenject;

using Object = UnityEngine.Object;

namespace BetterBeatSaber.Core.UI.SDK.Components; 

public sealed class TabSelector : Component {

    private static TextSegmentedControl? _segmentControlTemplate;

    private TextSegmentedControl? _textSegmentedControl;
    private TabSelectorBehaviour? _behaviour;

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
        // ReSharper disable once Unity.NoNullPropagation
        _behaviour?.Init(this, context);
    }

    // ReSharper disable once Unity.NoNullPropagation
    public void Refresh() => _behaviour?.Refresh();

    private sealed class TabSelectorBehaviour : MonoBehaviour {

        private TabSelector? TabSelector { get; set; }

        private readonly List<Tab> _tabs = new();

        private int _pageCount = -1;
        private int _currentPage;

        private bool _shouldRefresh;

        public int PageCount {
            get => _pageCount;
            set {
                _pageCount = value;
                if (_tabs.Count > 0) {
                    Refresh();
                }
            }
        }

        public void Init(TabSelector tabSelector, ParseContext parseContext) {

            TabSelector = tabSelector;
            
            _tabs.Clear();

            foreach (var tab in parseContext.GetComponentsByType<Tab>().Where(tab => tab.For == tabSelector.Id)) {
                _tabs.Add(tab);
                tab.Selector = tabSelector;
            }
            
            // Maybe add to left and to right buttons lols
            https://github.com/monkeymanboy/BeatSaberMarkupLanguage/blob/master/BeatSaberMarkupLanguage/Components/TabSelector.cs#L41

            Refresh();

            if (TabSelector._textSegmentedControl == null)
                return;
            
            TabSelector._textSegmentedControl.didSelectCellEvent -= TabSelected;
            TabSelector._textSegmentedControl.didSelectCellEvent += TabSelected;
            
            TabSelector._textSegmentedControl.SelectCellWithNumber(0);
            
            TabSelected(TabSelector._textSegmentedControl, 0);
            
        }

        public void Refresh() {
            
            if (!isActiveAndEnabled) {
                _shouldRefresh = true;
                return;
            }

            _shouldRefresh = false;
            
            var visibleTabs = _tabs.Where(tab => tab.Visible).ToList();
            
            if (PageCount == -1) {
                SetSegmentedControlTexts(visibleTabs);
            } else {
                if (_currentPage < 0)
                    _currentPage = 0;

                if (_currentPage > (visibleTabs.Count - 1) / _pageCount)
                    _currentPage = (visibleTabs.Count - 1) / _pageCount;

                SetSegmentedControlTexts(visibleTabs.Skip(PageCount * _currentPage).Take(PageCount).ToList());

                TabSelected(null, 0);
                
            }
            
        }

        private void TabSelected(SegmentedControl? segmentedControl, int index) {
            
            if (PageCount != -1)
                index += PageCount * _currentPage;

            foreach (var tab in _tabs)
                // ReSharper disable once Unity.NoNullPropagation
                tab.GameObject?.SetActive(false);

            if (index >= _tabs.Count(tab => tab.Visible))
                return;

            // ReSharper disable once Unity.NoNullPropagation
            _tabs.Where(tab => tab.Visible).ElementAt(index).GameObject?.SetActive(true);
            
        }

        private void SetSegmentedControlTexts(IEnumerable<Tab> tabs) {
            // ReSharper disable once Unity.NoNullPropagation
            TabSelector?._textSegmentedControl?.SetTexts(tabs.Select(tab => tab.Name).ToArray());
        }

        private void PageLeft() {
            _currentPage--;
            Refresh();
        }

        private void PageRight() {
            _currentPage++;
            Refresh();
        }

        private void OnEnable() {
            if (_shouldRefresh) {
                Refresh();
            }
        }
        
    }

}