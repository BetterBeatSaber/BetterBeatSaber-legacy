using System;

using UnityEngine;

using Object = UnityEngine.Object;

namespace BetterBeatSaber.Core.Utilities; 

// ReSharper disable MemberCanBeProtected.Global

public abstract class Singleton<T> where T : Singleton<T> {
    
    public static T Instance { get; private set; } = null!;

    protected Singleton() {
        if (Instance != null)
            throw new Exception("Only one instance can exist at a time!");
        Instance = (T) this;
    }

}

public abstract class UnitySingleton<T> : MonoBehaviour where T : UnitySingleton<T> {

    public static T Instance { get; private set; } = null!;

    protected UnitySingleton() {
        if (Instance != null)
            throw new Exception("Only one instance can exist at a time!");
        Instance = (T) this;
    }

    public void Destroy() {
        Object.Destroy(gameObject);
    }

    public static T Instantiate() {
        var gameObject = new GameObject(nameof(T));
        DontDestroyOnLoad(gameObject);
        return gameObject.AddComponent<T>();
    }

}

public abstract class ConstructableSingleton<T> where T : ConstructableSingleton<T> {

    private static T? _instance;
    public static T Instance {
        get { return _instance ??= Activator.CreateInstance<T>(); }
    }

}

public abstract class LockedConstructableSingleton<T> where T : LockedConstructableSingleton<T> {

    // ReSharper disable once StaticMemberInGenericType
    private static readonly object Lock = new();
    
    private static T? _instance;
    public static T Instance {
        get {
            lock (Lock) {
                return _instance ??= Activator.CreateInstance<T>();
            }
        }
    }

}