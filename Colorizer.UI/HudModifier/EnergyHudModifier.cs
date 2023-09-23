﻿using System.Collections;
using System.Collections.Generic;

using BetterBeatSaber.Colorizer.UI.Config;
using BetterBeatSaber.Core.Extensions;
using BetterBeatSaber.Core.Provider;
using BetterBeatSaber.Core.Utilities;

using HMUI;

using IPA.Utilities;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

namespace BetterBeatSaber.Colorizer.UI.HudModifier; 

public sealed class EnergyHudModifier : HudModifier {

    #region Injections

    [Inject]
    private readonly IGameEnergyCounter _energyCounter = null!;
    
    [Inject]
    private readonly GameEnergyUIPanel _gameEnergyUIPanel = null!;
        
    [Inject]
    private readonly ComboController _comboController = null!;
    
    [Inject]
    private readonly GameplayModifiers _gameplayModifiers = null!;
    
    [Inject]
    private readonly AssetProvider _assetProvider = null!;

    #endregion
    
    private Image? _energyBar;

    #region Shaking
    
    private Vector3 _originalPosition;
    
    private bool _shaking;
    private float _shakingIntensity;
    private float _shakingDuration;
    private bool _isDead;

    #endregion
    
    public void Start() {
        StartCoroutine(PrepareColorsForEnergyType(_gameplayModifiers.energyType));
    }

    public void OnDestroy() {
        _energyCounter.gameEnergyDidChangeEvent -= OnGameEnergyDidChangeEvent;
        if(UIColorizerConfig.Instance.EnergyHudModifier.ShakeOnComboBreak)
            _comboController.comboBreakingEventHappenedEvent -= OnComboBreakingEventHappenedEvent;
    }
    
    public void Update() {
        
        if (_energyBar == null)
            return;

        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (_gameplayModifiers.instaFail || (_energyCounter.energy == 1f && _gameplayModifiers.energyType == GameplayModifiers.EnergyType.Bar)) {
            _energyBar.color = RGB.Color0;
        }

    }
    
    private IEnumerator PrepareColorsForEnergyType(GameplayModifiers.EnergyType type) {
        
        yield return new WaitUntil(() => _gameEnergyUIPanel != null);
        
        if (type == GameplayModifiers.EnergyType.Battery) {
            
            var field = _gameEnergyUIPanel.GetField<List<Image>, GameEnergyUIPanel>("_batteryLifeSegments");
            
            field[0].color = Color.red;
            field[1].color = Color.red.LerpHSV(Color.yellow, .34f);
            field[2].color = Color.yellow.LerpHSV(Color.green, .66f);
            //field[1].color = HSBColor.Lerp(HSBColor.FromColor(Color.red), HSBColor.FromColor(Color.yellow), .34f).ToColor();
            //field[2].color = HSBColor.Lerp(HSBColor.FromColor(Color.yellow), HSBColor.FromColor(Color.green), .66f).ToColor();
            field[3].color = Color.green;

            if (!UIColorizerConfig.Instance.EnergyHudModifier.Glow)
                yield break;
            
            foreach (var f in field)
                f.material = _assetProvider.DistanceFieldMaterial;

            yield break;
        }

        if (type == GameplayModifiers.EnergyType.Bar) {
            if (_gameplayModifiers.instaFail) {
                _energyBar = _gameEnergyUIPanel.transform.Find("BatteryLifeSegment(Clone)").GetComponent<ImageView>();
                if (UIColorizerConfig.Instance.EnergyHudModifier.Glow) {
                    _energyBar.material = _assetProvider.DistanceFieldMaterial;
                }
                _energyBar.color = Color.green;
            } else {
                _energyBar = _gameEnergyUIPanel.transform.Find("EnergyBarWrapper/EnergyBar").GetComponent<ImageView>();
                if (UIColorizerConfig.Instance.EnergyHudModifier.Glow) {
                    _energyBar.material = _assetProvider.DistanceFieldMaterial;
                }
                _energyBar.color = Color.yellow;
            }
        }

        _originalPosition = _gameEnergyUIPanel.transform.position;
        
        _energyCounter.gameEnergyDidChangeEvent += OnGameEnergyDidChangeEvent;
        
        if(UIColorizerConfig.Instance.EnergyHudModifier.ShakeOnComboBreak)
            _comboController.comboBreakingEventHappenedEvent += OnComboBreakingEventHappenedEvent;

    }
    
    private IEnumerator Shake() {
        var elapsed = 0f;
        while (elapsed < _shakingDuration) {
            var newPosition = Random.insideUnitSphere * (Time.deltaTime * _shakingIntensity);
            var position = _gameEnergyUIPanel.transform.position;
            newPosition.z = position.z;
            //newPosition.y = position.y - Random.Range(-.5f, .5f);
            newPosition.y = position.y - Random.Range(-.025f, .025f);
            _gameEnergyUIPanel.transform.position = newPosition;
            elapsed += Time.deltaTime;
            yield return 0;
        }
        _shaking = false;
        _gameEnergyUIPanel.transform.position = _originalPosition;
    }

    private void OnComboBreakingEventHappenedEvent() {
        if (_shaking) {
            _shakingIntensity += 2.5f;
            _shakingDuration += .2f;
            //_shakingIntensity += 50f;
            //_shakingDuration += 50f;
            return;
        }
        _shaking = true;
        _shakingIntensity = 10f;
        _shakingDuration = .25f;
        //_shakingIntensity = 250f;
        //_shakingDuration = 10f;
        StartCoroutine(Shake());
    }
    
    private void OnGameEnergyDidChangeEvent(float energy) {
        if (_energyBar == null)
            return;
        _energyBar.color = energy switch {
            .5f => Color.yellow,
            > .5f => Color.yellow.LerpHSV(Color.green, (energy - .5f) * 2f),
            < .5f => Color.red.LerpHSV(Color.yellow, energy * 2f),
            _ => _energyBar.color
        };
        if (energy > 0f || !UIColorizerConfig.Instance.EnergyHudModifier.ShakeOnComboBreak || _isDead)
            return;
        _comboController.comboBreakingEventHappenedEvent -= OnComboBreakingEventHappenedEvent;
        _isDead = true;
    }
    
    public partial class Options : BaseOptions {

        public bool ShakeOnComboBreak { get; set; } = true;
        
    }

}