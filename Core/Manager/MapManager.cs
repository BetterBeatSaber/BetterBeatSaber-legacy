using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BetterBeatSaber.Core.Api;
using BetterBeatSaber.Core.Extensions;
using BetterBeatSaber.Core.Game;
using BetterBeatSaber.Shared.Models;
using BetterBeatSaber.Shared.Serialization;

using SongCore;

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace BetterBeatSaber.Core.Manager; 

public sealed class MapManager : Manager<MapManager> {

    #region Cache

    private uint[] _keyCache;
    private string[] _hashCache;
    
    public Song[] SongCache { get; private set; }
    
    #endregion
    
    public IEnumerable<CustomPreviewBeatmapLevel> Songs { get; private set; } = Enumerable.Empty<CustomPreviewBeatmapLevel>();
    
    public Song? CurrentSong => GetByHash(BeatSaber.CurrentMapHash);

    public bool IsLoading { get; set; }

    private readonly Dictionary<string, Sprite> _mapCoverCache = new();

    public event Action? OnSongLoading;
    public event Action? OnSongsLoaded;

    public event Action? OnCacheUpdating;
    public event Action? OnCacheUpdated;
    
    #region Init & Exit

    public override void Init() {
        
        Loader.LoadingStartedEvent += OnLoadingStartedEvent;
        Loader.SongsLoadedEvent += OnSongsLoadedEvent;

        AsyncHelper.RunSync(LoadCache);

    }

    public override void Exit() {
        Loader.SongsLoadedEvent -= OnSongsLoadedEvent;
        Loader.LoadingStartedEvent -= OnLoadingStartedEvent;
    }

    #endregion

    #region Event Handlers

    private void OnLoadingStartedEvent(Loader _) {
        OnSongLoading?.Invoke();
    }
    
    private void OnSongsLoadedEvent(Loader _, ConcurrentDictionary<string, CustomPreviewBeatmapLevel> songs) {
        OnSongsLoaded?.Invoke();
        Songs = songs.Values;
        Logger.Info("{0} songs downloaded/installed", songs.Count);
    }

    #endregion

    #region Methods

    #region Public
    
    public async Task LoadCache() {
        
        OnCacheUpdating?.Invoke();
        
        var response = await ApiClient.Instance.GetRaw("/maps/download/cache");
        if (response == null) {
            Logger.Error("Failed to download cache");
            return;
        }
        
        var data = await response.Content.ReadAsByteArrayAsync();
        if (data == null) {
            Logger.Error("Failed to read cache");
            return;
        }
        
        var buffer = new ByteBuffer(data);
        
        SongCache = buffer.ReadArray<Song>();
        
        _keyCache = SongCache.Select(song => Convert.ToUInt32(song.Key, 16)).ToArray();
        _hashCache = SongCache.Select(song => song.Hash).ToArray();

        OnCacheUpdated?.Invoke();
        
        Logger.Info("Cached {0} songs", SongCache.Length);
        
    }

    #region Get

    public Song? GetByDifficultyBeatmap(IDifficultyBeatmap? difficultyBeatmap) =>
        difficultyBeatmap != null ? GetByHash(difficultyBeatmap.GetHash()) : null;
    
    public Song? GetByHash(string hash) {
        var index = Array.BinarySearch(_hashCache, hash);
        if(index < 0)
            return null;
        return SongCache[index];
    }

    public Song? GetByKey(string key) =>
        GetByKey(Convert.ToUInt32(key, 16));

    public Song? GetByKey(uint key) {
        var index = Array.BinarySearch(_keyCache, key);
        if(index < 0)
            return null;
        return SongCache[index];
    }
    
    #endregion

    public void DownloadMapSync(string hash) =>
        AsyncHelper.RunSync(async () => await DownloadMap(hash));
    
    public void DownloadMapByHashSync(string hash) =>
        AsyncHelper.RunSync(async () => await DownloadMapByHash(hash));
    
    public async Task DownloadMap(string hash) {
        
    }
    
    public async Task DownloadMapByHash(string hash) {
        
    }
    
    public IEnumerator DownloadMapCoverAndApply(string hash, Image image) {

        if (_mapCoverCache.TryGetValue(hash, out var avatar)) {
            image.sprite = avatar;
            yield break;
        }

        yield return DownloadMapCover(hash);

        image.sprite = _mapCoverCache[hash];
        
    }

    #endregion
    
    #region Private

    private IEnumerator DownloadMapCover(string hash) {
        
        var handler = new DownloadHandlerTexture();
        var request = new UnityWebRequest($"https://cdn.beatsaver.com/{hash.ToLower()}.jpg", "GET");
        
        request.downloadHandler = handler;

        yield return request.SendWebRequest();
        
        _mapCoverCache.Add(hash, Sprite.Create(handler.texture, new Rect(0, 0, handler.texture.width, handler.texture.height), new Vector2(.5f, .5f)));
        
    }
    
    #endregion
    
    #endregion

}