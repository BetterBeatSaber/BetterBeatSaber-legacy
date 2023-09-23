using System.Collections;
using System.Collections.Generic;
using System.Linq;

using BetterBeatSaber.Core.Extensions;
using BetterBeatSaber.Core.Game;

using SongDetailsCache;
using SongDetailsCache.Structs;

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace BetterBeatSaber.Core.Manager; 

public sealed class MapManager : Manager<MapManager> {

    #region Properties

    public Song? CurrentSong => GetByHash(BeatSaber.CurrentMapHash);

    #endregion

    #region Fields

    private readonly Dictionary<string, Sprite> _mapCoverCache = new();

    private SongDetails? _songDetails;

    #endregion
    
    #region Init & Exit

    public override void Init() {
        _songDetails = AsyncHelper.RunSync(SongDetails.Init);
        SongDetailsContainer.dataAvailableOrUpdated += OnDataAvailableOrUpdated;
    }

    public override void Exit() {
        SongDetailsContainer.dataAvailableOrUpdated -= OnDataAvailableOrUpdated;
    }

    #endregion

    #region Event Handlers

    private void OnDataAvailableOrUpdated() {
        _songDetails = AsyncHelper.RunSync(SongDetails.Init);
    }

    #endregion

    #region Methods

    public Song? GetByDifficultyBeatmap(IDifficultyBeatmap? difficultyBeatmap) {
        return difficultyBeatmap != null ? GetByHash(difficultyBeatmap.GetHash()) : null;
    }

    public Song? GetByHash(string? hash) {
        
        if (hash == null || _songDetails == null)
            return null;

        _songDetails.songs.FindByHash(hash, out var song);
        
        return song;
        
    }

    public Song? GetByKey(string? key) {
        
        if (key == null || _songDetails == null)
            return null;

        _songDetails.songs.FindByMapId(key, out var song);
        
        return song;
        
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

    #region Private Methods

    private IEnumerator DownloadMapCover(string hash) {
        
        var handler = new DownloadHandlerTexture();
        var request = new UnityWebRequest($"https://cdn.beatsaver.com/{hash.ToLower()}.jpg", "GET");
        
        request.downloadHandler = handler;

        yield return request.SendWebRequest();
        
        _mapCoverCache.Add(hash, Sprite.Create(handler.texture, new Rect(0, 0, handler.texture.width, handler.texture.height), new Vector2(.5f, .5f)));
        
    }
    
    #endregion

}