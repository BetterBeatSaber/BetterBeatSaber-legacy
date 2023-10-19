using System;
using System.IO;
using System.Linq;

using BetterBeatSaber.Core.Extensions;
using BetterBeatSaber.Core.Game.Enums;
using BetterBeatSaber.Core.Manager.Interop;
using BetterBeatSaber.Core.Utilities;
using BetterBeatSaber.Core.Zenject;
using BetterBeatSaber.Shared.Models;
using BetterBeatSaber.Shared.Network.Enums;

using JetBrains.Annotations;

using UnityEngine;
using UnityEngine.SceneManagement;

using Valve.VR;

using Zenject;

namespace BetterBeatSaber.Core.Game; 

// ReSharper disable Unity.NoNullPropagation

public class BeatSaber : UnitySingleton<BeatSaber> {

    #region Directories

    public static string GameDirectory => Environment.CurrentDirectory;
    
    public static string ManagerDirectory => Path.Combine(GameDirectory, "Beat Saber_Data", "Managed");
    public static string UserDataDirectory => Path.Combine(GameDirectory, "UserData");
    public static string PluginsDirectory => Path.Combine(GameDirectory, "Plugins");
    public static string LibrariesDirectory => Path.Combine(GameDirectory, "Libs");

    public static string DataDirectory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "LocalLow", "Hyperbolic Magnetism", "Beat Saber");
    
    public static string CustomSabersDirectory => Path.Combine(GameDirectory, "CustomSabers");
    public static string CustomNotesDirectory => Path.Combine(GameDirectory, "CustomNotes");

    #endregion
    
    /// <summary>
    /// Current game version
    /// </summary>
    public static Hive.Versioning.Version Version => Hive.Versioning.Version.Parse(Application.version.Replace("_", "-"));

    /// <summary>
    /// True if the game was started with the "fpfc" flag
    /// </summary>
    public static readonly bool InDesktopMode = Environment.GetCommandLineArgs().Contains("fpfc");

    #region Variables

    /// <summary>
    /// Current active generic scene
    /// </summary>
    public static GenericScene ActiveGenericScene { get; private set; }

    /// <summary>
    /// True if the player is currently watching a replay
    /// </summary>
    public static bool IsWatchingReplay { get; private set; }
    
    /// <summary>
    /// True if the player is currently playing a map
    /// </summary>
    public static bool IsPlayingLevel { get; private set; }

    /// <summary>
    /// True if the player is wearing the HMD
    /// </summary>
    public static bool IsImmersed { get; private set; }

    public static IDifficultyBeatmap? CurrentDifficultyBeatmap => PlayerBinding.GameplayCoreSceneSetupData?.difficultyBeatmap;
    public static string? CurrentMapHash => CurrentDifficultyBeatmap?.level.levelID.Substring("custom_level_".Length).ToLower();

    public static bool IsPaused { get; private set; }
    public static Rank? CurrentRank { get; private set; }
    public static float? CurrentScore { get; private set; }

    #endregion

    #region Events

    #region HMD

    /// <summary>
    /// Will be called when the HMD has been put on (Only if not in fpfc and with a maximum delay of 500ms)
    /// </summary>
    public static event Action? OnHmdImmerse;
    
    /// <summary>
    /// Will be called when the HMD has been taken off (Only if not in fpfc and with a maximum delay of 500ms)
    /// </summary>
    public static event Action? OnHmdEmerge;
    
    #endregion
    
    #region Scenes

    public static event Action<Scene, Scene>? OnSceneChanged;
    public static event Action<GenericScene>? OnGenericSceneChanged;

    #endregion
    
    #region Level

    /// <summary>
    /// Will be called when playing a Level (Not watching a Replay)
    /// </summary>
    public static event Action<GameplayCoreSceneSetupData>? OnLevelStarted;
    
    /// <summary>
    /// Will be called when returning to the Menu (After playing a Level; Not watching an Replay)
    /// </summary>
    public static event Action? OnLevelEnded;
    
    /// <summary>
    /// Will be called when the Level has been finished
    /// </summary>
    public static event Action? OnLevelFinished;
    
    /// <summary>
    /// Will be called when the Level has been failed
    /// </summary>
    public static event Action? OnLevelFailed;
    
    /// <summary>
    /// Will be called when the Player quits the Level
    /// </summary>
    public static event Action? OnLevelQuit;
    
    /// <summary>
    /// Will be called when the Player pauses the Level
    /// </summary>
    public static event Action? OnLevelPaused;
    
    /// <summary>
    /// Will be called when the Player unpauses/resumes the Level
    /// </summary>
    public static event Action? OnLevelUnpaused;

    /*public static event Action<RankModel.Rank>? OnRankUpdated;

    public static event Action<float>? OnScoreUpdated;*/

    #endregion
    
    #region Replay

    /// <summary>
    /// Will be called when a Replay has been started
    /// </summary>
    public static event Action<IDifficultyBeatmap, Replay>? OnReplayStarted;
    
    /// <summary>
    /// Will be either called when a Replay has finished or when returned to the main menu after a Replay
    /// </summary>
    public static event Action<IDifficultyBeatmap, Replay>? OnReplayFinished;

    #endregion

    #endregion

    #region Private

    private static bool _isReplay;

    #endregion
    
    #region Unity Event Functions

    private void Start() {
        
        ZenjectManager.Instance.OnInstall += OnZenjectInstall;

        SceneManager.activeSceneChanged += OnActiveSceneChanged;

        InteropManager.Instance.OnReplayStarted += (beatmap, replay) => {
            _isReplay = true;
            OnReplayStarted?.Invoke(beatmap, replay);
        };

        InteropManager.Instance.OnReplayEnded += (beatmap, replay) => {
            _isReplay = false;
            OnReplayFinished?.Invoke(beatmap, replay);
        };
        
        /*ReplayerLauncher.ReplayWasStartedEvent += data => {
            _isReplay = true;
            OnReplayStarted?.Invoke(data.DifficultyBeatmap, new Replay {
                Map = new Map {
                    Type = MapType.Custom,
                    SongName = data.DifficultyBeatmap.level.songName,
                    SongAuthor = data.DifficultyBeatmap.level.songAuthorName,
                    LevelAuthor = data.DifficultyBeatmap.level.levelAuthorName,
                    Hash = data.DifficultyBeatmap.GetHash()
                },
                User = new User {
                    PlatformId = data.MainReplay.ReplayData.Player?.id ?? "unknown",
                    Name = data.MainReplay.ReplayData.Player?.name ?? "No Name",
                    AvatarUrl = data.MainReplay.ReplayData.Player?.avatar ?? "none"
                }
            });
        };

        ReplayerLauncher.ReplayWasFinishedEvent += data => {
            _isReplay = false;
            OnReplayFinished?.Invoke(data.DifficultyBeatmap, new Replay {
                Map = new Map {
                    Type = MapType.Custom,
                    SongName = data.DifficultyBeatmap.level.songName,
                    SongAuthor = data.DifficultyBeatmap.level.songAuthorName,
                    LevelAuthor = data.DifficultyBeatmap.level.levelAuthorName,
                    Hash = data.DifficultyBeatmap.GetHash()
                },
                User = new User {
                    PlatformId = data.MainReplay.ReplayData.Player?.id ?? "unknown",
                    Name = data.MainReplay.ReplayData.Player?.name ?? "No Name",
                    AvatarUrl = data.MainReplay.ReplayData.Player?.avatar ?? "none"
                }
            });
        };*/
        
        if(!InDesktopMode)
            InvokeRepeating(nameof(CheckIfHmdIsImmersed), 1f, .5f);
        
    }

    private void OnDestroy() {
        
        ZenjectManager.Instance.OnInstall -= OnZenjectInstall;
        
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        
    }

    #endregion

    #region Methods

    public static void Restart() =>
        Resources.FindObjectsOfTypeAll<MenuTransitionsHelper>().FirstOrDefault()?.RestartGame();

    #endregion

    #region Private Methods

    private void CheckIfHmdIsImmersed() {
        switch (SteamVR_Controller.Input(0).GetPress(EVRButtonId.k_EButton_ProximitySensor)) {
            case true when !IsImmersed:
                IsImmersed = true;
                OnHmdImmerse?.Invoke();
                break;
            case false when IsImmersed:
                IsImmersed = false;
                OnHmdEmerge?.Invoke();
                break;
        }
    }

    #endregion
    
    #region Event Handlers

    private static void OnZenjectInstall(Location location, DiContainer container) {
        
        if (!location.IsPlayer())
            return;
        
        container.BindInterfacesAndSelfTo<PlayerBinding>().AsSingle();

        if (!_isReplay)
            OnLevelStarted?.Invoke(container.TryResolve<GameplayCoreSceneSetupData>());
        
    }
    
    private static void OnActiveSceneChanged(Scene from, Scene to) {
        
        OnSceneChanged?.Invoke(from, to);

        if (from.name == "GameCore" && to.name == "MainMenu") {
            OnLevelEnded?.Invoke();
            IsPaused = false;
        }

        var genericScene = to.name switch {
            "MainMenu" => GenericScene.Menu,
            "GameCore" => GenericScene.Game,
            _ => GenericScene.None
        };

        ActiveGenericScene = genericScene;
        
        if(genericScene != GenericScene.None)
            OnGenericSceneChanged?.Invoke(genericScene);
        
    }

    #endregion

    internal sealed class PlayerBinding : IInitializable, IDisposable {

        #region Data to be accessed

        internal static GameplayCoreSceneSetupData? GameplayCoreSceneSetupData;
        internal static AudioTimeSyncController? AudioTimeSyncController;
        internal static PauseController? PauseController;
        internal static RelativeScoreAndImmediateRankCounter? RelativeScoreAndImmediateRankCounter;
        
        #endregion
        
        #region Injections

        [UsedImplicitly]
        [Inject]
        private readonly GameplayCoreSceneSetupData _gameplayCoreSceneSetupData = null!;
        
        [UsedImplicitly]
        [Inject]
        private readonly AudioTimeSyncController _audioTimeSyncController = null!;
        
        [UsedImplicitly]
        [Inject]
        private readonly PauseController _pauseController = null!;
        
        [UsedImplicitly]
        [Inject]
        private readonly RelativeScoreAndImmediateRankCounter _relativeScoreAndImmediateRankCounter = null!;

        #endregion
        
        public void Initialize() {

            GameplayCoreSceneSetupData = _gameplayCoreSceneSetupData;
            PauseController = _pauseController;
            AudioTimeSyncController = _audioTimeSyncController;
            RelativeScoreAndImmediateRankCounter = _relativeScoreAndImmediateRankCounter;
            
            _pauseController.didPauseEvent += OnInternalLevelPaused;
            _pauseController.didResumeEvent += OnInternalLevelUnpaused;
            
            _relativeScoreAndImmediateRankCounter.relativeScoreOrImmediateRankDidChangeEvent += OnRelativeScoreOrImmediateRankDidChange;
            
        }

        #region Event Handlers

        private static void OnInternalLevelPaused() {
            IsPaused = true;
            OnLevelPaused?.Invoke();
        }
        
        private static void OnInternalLevelUnpaused() {
            IsPaused = false;
            OnLevelUnpaused?.Invoke();
        }
        
        private void OnRelativeScoreOrImmediateRankDidChange() {
            CurrentRank = _relativeScoreAndImmediateRankCounter.immediateRank.ToRank();
            CurrentScore = _relativeScoreAndImmediateRankCounter.relativeScore;
        }

        #endregion
        
        public void Dispose() {
            
            _pauseController.didPauseEvent -= OnInternalLevelPaused;
            _pauseController.didResumeEvent -= OnInternalLevelUnpaused;
            
            _relativeScoreAndImmediateRankCounter.relativeScoreOrImmediateRankDidChangeEvent -= OnRelativeScoreOrImmediateRankDidChange;
            
        }

    }

}