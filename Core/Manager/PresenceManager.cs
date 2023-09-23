using System;
using System.Collections;

using BetterBeatSaber.Core.Extensions;
using BetterBeatSaber.Core.Game;
using BetterBeatSaber.Core.Game.Enums;
using BetterBeatSaber.Core.Network;
using BetterBeatSaber.Core.Utilities;
using BetterBeatSaber.Shared.Enums;
using BetterBeatSaber.Shared.Models;
using BetterBeatSaber.Shared.Network.Enums;
using BetterBeatSaber.Shared.Network.Interfaces;
using BetterBeatSaber.Shared.Network.Packets;

using LiteNetLib;

using UnityEngine;

namespace BetterBeatSaber.Core.Manager; 

public sealed class PresenceManager : UnitySingleton<PresenceManager> {

    public IPresence PreviousPresence { get; private set; } = new OfflinePresence();
    
    private IPresence _presence = new InMenuPresence();
    public IPresence Presence {
        get => _presence;
        private set {

            // ReSharper disable once ReplaceWithSingleAssignment.True
            var invokeChange = true;

            if (_presence.Status == Shared.Enums.Status.InMenu && value.Status == Shared.Enums.Status.InMenu)
                invokeChange = false;

            if(_presence.Status != Shared.Enums.Status.Afk)
                PreviousPresence = _presence;
                
            _presence = value;

            if (!invokeChange)
                return;

            OnPresenceUpdated?.Invoke(value);
                
            NetworkClient.Instance.SendPacket(new PresencePacket {
                Presence = value
            });

        }
    }

    private Coroutine? _updatePlayingMapPresenceStateCoroutine;
    
    public event Action<IPresence>? OnPresenceUpdated;

    #region Unity Event Functions

    private void Start() {
        
        BeatSaber.OnGenericSceneChanged += OnGenericSceneChanged;
        
        BeatSaber.OnReplayStarted += OnReplayStarted;
        
        BeatSaber.OnLevelStarted += OnLevelStarted;
        BeatSaber.OnLevelEnded += OnLevelEnded;

        BeatSaber.OnLevelPaused += OnLevelPaused;
        BeatSaber.OnLevelUnpaused += OnLevelUnpaused;
        
        BeatSaber.OnHmdEmerge += OnHmdEmerge;
        BeatSaber.OnHmdImmerse += OnHmdImmerse;
        
        Presence = new InMenuPresence();
        
    }

    private void OnDestroy() {
        
        BeatSaber.OnGenericSceneChanged -= OnGenericSceneChanged;
        
        BeatSaber.OnReplayStarted -= OnReplayStarted;
        
        BeatSaber.OnLevelStarted -= OnLevelStarted;
        BeatSaber.OnLevelEnded -= OnLevelEnded;
        
        BeatSaber.OnLevelPaused -= OnLevelPaused;
        BeatSaber.OnLevelUnpaused -= OnLevelUnpaused;
        
        BeatSaber.OnHmdEmerge -= OnHmdEmerge;
        BeatSaber.OnHmdImmerse -= OnHmdImmerse;
        
    }

    #endregion
    
    #region Event Handlers

    private void OnGenericSceneChanged(GenericScene scene) {
        if (scene == GenericScene.Menu)
            Presence = new InMenuPresence();
    }
    
    private void OnReplayStarted(IDifficultyBeatmap difficultyBeatmap, Replay replay) {
        difficultyBeatmap.GetBeatmapDataBasicInfoAsync().ContinueWith(x => {
            Presence = new WatchingReplayPresence {
                Map = replay.Map,
                Difficulty = new DifficultyMap {
                    MapType = MapType.Custom,
                    Difficulty = difficultyBeatmap.GetMapDifficulty(),
                    NotesPerSecond = x.Result.cuttableNotesCount / difficultyBeatmap.level.beatmapLevelData.audioClip.length,
                    NoteJumpSpeed = difficultyBeatmap.noteJumpMovementSpeed,
                    BeatLeaderStars = 0,
                    ScoreSaberStars = 0
                },
                User = replay.User
            };
        });
    }
    
    private void OnLevelStarted(GameplayCoreSceneSetupData data) {
        
        _previousPausedAt = DateTime.Now;

        _updatePlayingMapPresenceStateCoroutine = StartCoroutine(UpdatePlayingMapPresenceState());

        data.difficultyBeatmap.GetBeatmapDataBasicInfoAsync().ContinueWith(x => {
            
            if (x.Result == null || x.IsFaulted)
                return;

            var mapType = data.difficultyBeatmap.GetMapType();
            
            Presence = new PlayingMapPresence {
                Map = new Map {
                    Type = mapType,
                    Hash = mapType == MapType.Custom ? data.difficultyBeatmap.GetHash() : null,
                    SongName = data.difficultyBeatmap.level.songName,
                    SongAuthor = data.difficultyBeatmap.level.songAuthorName,
                    LevelAuthor = data.difficultyBeatmap.level.levelAuthorName
                },
                Difficulty = new DifficultyMap {
                    MapType = mapType,
                    Difficulty = data.difficultyBeatmap.GetMapDifficulty(),
                    NotesPerSecond = x.Result.cuttableNotesCount / data.difficultyBeatmap.level.beatmapLevelData.audioClip.length,
                    NoteJumpSpeed = data.difficultyBeatmap.noteJumpMovementSpeed,
                    BeatLeaderStars = 0,
                    ScoreSaberStars = 0
                }
            };

        });
        
    }
    
    private void OnLevelEnded() {
        if (_updatePlayingMapPresenceStateCoroutine != null)
            StopCoroutine(_updatePlayingMapPresenceStateCoroutine);
    }

    private DateTime? _previousPausedAt;
    private DateTime? _pausedAt;
    
    private void OnLevelPaused() {
        
        if (Presence.Status != Shared.Enums.Status.PlayingMap || Presence is not PlayingMapPresence mapPresence)
            return;
        
    }
    
    private void OnLevelUnpaused() {
        if (Presence.Status == Shared.Enums.Status.PlayingMap && Presence is PlayingMapPresence mapPresence) {
        }
    }

    private void OnHmdImmerse() {
        Presence = PreviousPresence;
    }

    private void OnHmdEmerge() {
        Presence = new AfkPresence();
    }
    
    #endregion

    private static IEnumerator UpdatePlayingMapPresenceState() {
        while(true) {
            yield return new WaitForSeconds(.5f);
            NetworkClient.Instance.SendPacket(new PresenceStatePacket {
                PresenceState = new PlayingMapPresenceState {
                    Paused = BeatSaber.IsPaused,
                    Rank = BeatSaber.CurrentRank ?? Rank.None,
                    Score = BeatSaber.CurrentScore ?? 0f
                }
            }, DeliveryMethod.Unreliable);
        }
        // ReSharper disable once IteratorNeverReturns
    }
    
}