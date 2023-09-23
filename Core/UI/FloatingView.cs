using System;
using System.Collections.Generic;
using System.Linq;

using BeatSaberMarkupLanguage.FloatingScreen;

using BetterBeatSaber.Core.Game;
using BetterBeatSaber.Core.Game.Enums;

using JetBrains.Annotations;

using UnityEngine;

using VRUIControls;

namespace BetterBeatSaber.Core.UI;

public abstract class FloatingView : View {

    [UsedImplicitly]
    public FloatingViewConfigBase Config { get; set; } = null!;
    
    public FloatingScreen FloatingScreen => transform.parent.GetComponent<FloatingScreen>();

    public virtual FloatingScreen.Side Handle { get; } = FloatingScreen.Side.Full;
    
    public abstract Vector2 ScreenSize { get; }
    
    public bool ShowHandle {
        get => FloatingScreen.ShowHandle;
        set {

            if (value)
                FloatingScreen.HandleReleased += OnHandleReleased;
            else
                FloatingScreen.HandleReleased -= OnHandleReleased;
            
            FloatingScreen.ShowHandle = value;
            
        }
    }

    public FloatingScreen.Side HandleSide {
        get => FloatingScreen.HandleSide;
        set => FloatingScreen.HandleSide = value;
    }

    public Vector2 Size {
        get => FloatingScreen.ScreenSize;
        set => FloatingScreen.ScreenSize = value;
    }
    
    private bool _allowMovement;
    public bool AllowMovement {
        get => _allowMovement;
        set {
            
            _allowMovement = value;

            FloatingScreen.ShowHandle = value;
            
            // Refresh VR pointer due to bug
            if (!value)
                return;
            
            var pointers = Resources.FindObjectsOfTypeAll<VRPointer>();
            var pointer = BeatSaber.ActiveGenericScene == GenericScene.Game ? pointers.LastOrDefault() : pointers.FirstOrDefault();

            if (pointer == null)
                return;
                
            if (FloatingScreen.screenMover)
                Destroy(FloatingScreen.screenMover);

            FloatingScreen.screenMover = pointer.gameObject.AddComponent<FloatingScreenMoverPointer>();
            FloatingScreen.screenMover.Init(FloatingScreen);
            
        }
    }

    public void OnHandleReleased(object _, FloatingScreenHandleEventArgs args) {
        Config.Positions[BeatSaber.ActiveGenericScene] = args.Position;
        Config.Rotations[BeatSaber.ActiveGenericScene] = args.Rotation;
        SaveConfig();
    }

    public virtual void SaveConfig() {}
    
}

public abstract class FloatingView<T> : FloatingView where T : FloatingView<T> {

    public static T? Instance { get; private set; }

    protected FloatingView() {
        Instance = (T?) this;
    }

}

public class FloatingViewConfigBase {

    public static readonly Vector3 DefaultPosition = new(0f, 4.1f, 3.5f);
    public static readonly Quaternion DefaultRotation = Quaternion.Euler(325f, 0f, 0f);

    private static readonly Dictionary<GenericScene, Vector3> DefaultPositions = new() {
        { GenericScene.Menu, DefaultPosition },
        { GenericScene.Game, DefaultPosition }
    };
    
    private static readonly Dictionary<GenericScene, Quaternion> DefaultRotations = new() {
        { GenericScene.Menu, DefaultRotation },
        { GenericScene.Game, DefaultRotation }
    };
    
    public bool Enabled { get; set; } = false;
    
    public FloatingViewVisibility Visibility { get; set; } = FloatingViewVisibility.Everywhere;

    public Dictionary<GenericScene, Vector3> Positions { get; set; } = DefaultPositions;
    
    public Dictionary<GenericScene, Quaternion> Rotations { get; set; } = DefaultRotations;

    public bool ShouldBeVisible(GenericScene genericScene) =>
        Enabled && (
            Visibility == FloatingViewVisibility.Everywhere || 
            (Visibility == FloatingViewVisibility.Game && genericScene == GenericScene.Game) ||
            (Visibility == FloatingViewVisibility.Menu && genericScene == GenericScene.Menu));

    public Vector3 GetPosition(GenericScene genericScene) =>
        Positions.TryGetValue(genericScene, out var position) ? position : DefaultPosition;

    public Quaternion GetRotation(GenericScene genericScene) =>
        Rotations.TryGetValue(genericScene, out var rotation) ? rotation : DefaultRotation;

    public void ResetPositionsAndRotations() {
        Positions = DefaultPositions;
        Rotations = DefaultRotations;
    }

}

[Flags]
public enum FloatingViewVisibility : byte {

    Everywhere = Menu | Game,
    
    Menu = 0x01,
    Game = 0x02

}