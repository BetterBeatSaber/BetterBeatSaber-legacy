using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.FloatingScreen;
using BeatSaberMarkupLanguage.GameplaySetup;

using BetterBeatSaber.Core.Extensions;
using BetterBeatSaber.Core.Game;
using BetterBeatSaber.Core.Game.Enums;
using BetterBeatSaber.Core.Manager;
using BetterBeatSaber.Core.Zenject;

using HMUI;

using IPA.Utilities;

using ModestTree;

using UnityEngine;

using Zenject;

namespace BetterBeatSaber.Core.UI; 

public sealed class UIManager : Manager<UIManager> {

    private readonly List<Type> _modifierViewTypes = new();
    private readonly List<ModifierView> _modifierViews = new();

    private readonly Dictionary<Type, FloatingViewConfigBase> _floatingScreenTypes = new();
    
    #region Init & Exit

    public override void Init() {
        ZenjectManager.Instance.OnInstall += OnInstall;
        BeatSaber.OnGenericSceneChanged += OnGenericSceneChanged;
    }

    public override void Exit() {
        ZenjectManager.Instance.OnInstall -= OnInstall;
        BeatSaber.OnGenericSceneChanged -= OnGenericSceneChanged;
    }

    #endregion

    #region Event Handlers

    private void OnGenericSceneChanged(GenericScene scene) {
        foreach (var (type, config) in _floatingScreenTypes) {

            var viewController = type.GetInstance<FloatingView>();
            
            FloatingScreen? floatingScreen;
            if (viewController != null && viewController.FloatingScreen != null) {

                floatingScreen = viewController.FloatingScreen;
                
                if (!config.ShouldBeVisible(scene)) {
                    if(floatingScreen.gameObject.activeSelf)
                        floatingScreen.gameObject.SetActive(false);
                    continue;
                }
                
                if(!floatingScreen.gameObject.activeSelf)
                    floatingScreen.gameObject.SetActive(true);
                
                floatingScreen.ScreenPosition = config.GetPosition(scene);
                floatingScreen.ScreenRotation = config.GetRotation(scene);
                
                continue;
                
            }

            if (!config.ShouldBeVisible(scene))
                continue;

            if (viewController == null)
                viewController = (FloatingView?) CreateViewController(type);

            if (viewController == null)
                continue;
            
            viewController.Config = config;
            
            floatingScreen = FloatingScreen.CreateFloatingScreen(viewController.ScreenSize, true, config.GetPosition(scene), config.GetRotation(scene));

            floatingScreen.HandleSide = viewController.Handle;
            
            floatingScreen.SetRootViewController(viewController, ViewController.AnimationType.None);

            viewController.ShowHandle = false;
            
            floatingScreen.gameObject.name = type.Name;
            
        }
    }

    private void OnInstall(Location location, DiContainer container) {
        
        if(!location.HasFlag(Location.Menu))
            return;
        
        foreach (var modifierViewType in _modifierViewTypes) {
            
            var modifierView = modifierViewType.Construct<ModifierView>();
            if (modifierView == null)
                return;

            var resource = modifierViewType.FullName!.Substring(0, modifierViewType.FullName.LastIndexOf('.')) + ".modifier-view.bsml";

            typeof(GameplaySetup)
                .GetMethod("AddTab", BindingFlags.NonPublic | BindingFlags.Instance)!
                .Invoke(GameplaySetup.instance, new object[] { modifierViewType.Assembly, modifierView.Title, resource, modifierView, modifierView.MenuType });
            
            _modifierViews.Add(modifierView);
            
        }

    }
    
    #endregion

    #region Modifier Views

    public void RegisterModifierView<T>() where T : ModifierView =>
        RegisterModifierView(typeof(T));

    public void RegisterModifierView(Type type) =>
        _modifierViewTypes.Add(type);

    public void UnregisterModifierView<T>() where T : ModifierView =>
        UnregisterModifierView(typeof(T));

    public void UnregisterModifierView(Type type) {
        foreach (var modifierView in _modifierViews.ToList().Where(modifierView => modifierView.GetType().FullName == type.FullName)) {
            GameplaySetup.instance.RemoveTab(modifierView.Title);
            _modifierViews.Remove(modifierView);
            _modifierViewTypes.Remove(modifierView.GetType());
        }
    }
    
    #endregion

    #region Floating Views

    public bool RegisterFloatingView<T>(FloatingViewConfigBase? config) where T : FloatingView<T> {
        
        if (config == null)
            return false;
        
        _floatingScreenTypes.Add(typeof(T), config);

        return true;

    }
    
    public bool UnregisterFloatingView<T>() where T : FloatingView<T> =>
        _floatingScreenTypes.Remove(typeof(T));

    #endregion
    
    #region Extend BSML BeatSaberUI

    public static FlowController? CreateFlowController(Type? type, bool showBackButton = true) =>
        (FlowController?) CreateFlowCoordinator(type, showBackButton);
    
    public static FlowCoordinator? CreateFlowCoordinator(Type? type, bool showBackButton = true) {
        
        if (type == null)
            return null;
        
        var method = typeof(BeatSaberUI).GetMethod(nameof(BeatSaberUI.CreateFlowCoordinator), BindingFlags.Public | BindingFlags.Static);
        if (method == null)
            return null;
        
        var genericMethod = method.MakeGenericMethod(type);
        
        var flowCoordinator = (FlowCoordinator?) genericMethod.Invoke(null, Array.Empty<object>());
        if (flowCoordinator == null)
            return null;
        
        if(showBackButton)
            flowCoordinator.SetField("_showBackButton", true);
        
        return flowCoordinator;
        
    }

    public static ViewController? CreateViewController(Type? type) {
        
        if (type == null)
            return null;
        
        var method = typeof(BeatSaberUI).GetMethod(nameof(BeatSaberUI.CreateViewController), BindingFlags.Public | BindingFlags.Static);
        if (method == null)
            return null;
        
        var genericMethod = method.MakeGenericMethod(type);
        
        var viewController = (ViewController?) genericMethod.Invoke(null, Array.Empty<object>());
        
        return viewController == null ? null : viewController;

    }

    #endregion

    #region Popups / Modals

    public Popup.Popup? ShowPopup(string title, string text, string? buttonText = null, Action? buttonClickHandler = null, GameObject? parent = null) {

        if (BeatSaber.ActiveGenericScene != GenericScene.Menu)
            return null;

        parent ??= BeatSaberUI.MainFlowCoordinator.GetField<ViewController, FlowCoordinator>("_providedMainViewController").gameObject;

        var popup = BeatSaberUI.CreateViewController<Popup.Popup>();
        
        var parserParams = BSMLParser.instance.Parse(typeof(Popup.Popup).ReadViewDefinition(), parent, popup);

        popup.SetTitle(title);
        popup.SetText(text);

        if (buttonText != null) {
            
            // if buttonClickHandler null just close else run the handler
        }
        
        parserParams.EmitEvent("Show");
        
        return popup;

    }
    
    #endregion

    #region Parse

    #if !DEBUG
    // ReSharper disable once CollectionNeverUpdated.Local
    private static readonly Dictionary<Type, string> Cache = new();
    #endif

    internal const string Fallback = "<text text=\"Resource not found\" align=\"Center\"/>";
    
    private T Parse<T>() {

        var d = BeatSaberUI.CreateViewController<View>();
        
        
        return default;
    }

    private void Parse2() {
        
        
    }

    internal static string ReadViewDefinition<T>() where T : View =>
        ReadViewDefinition(typeof(T));

    internal static string ReadViewDefinition(Type type) {

        #if !DEBUG
        if (Cache.TryGetValue(type, out var definition) && definition != null)
            return definition;
        #endif

        using var stream = type.Assembly.GetManifestResourceStream(GetViewDefinitionResourceName(type));
        if (stream == null)
            return Fallback;
        
        using var streamReader = new StreamReader(stream);
        var resource = streamReader.ReadToEnd();

        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (resource.IsEmpty())
            return Fallback;
        
        #if !DEBUG
        Cache[type] = resource;
        #endif
        
        return resource;
        
    }
    
    public static string GetViewDefinitionResourceName(Type type) =>
        string.Join(".", type.Namespace, type.Name, "bsml");

    #endregion
    
}