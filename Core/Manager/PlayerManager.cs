using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BetterBeatSaber.Core.Api;
using BetterBeatSaber.Shared.Enums;
using BetterBeatSaber.Shared.Models;

using ModestTree;

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace BetterBeatSaber.Core.Manager; 

// TODO: Fetch friend avatars

public sealed class PlayerManager : Manager<PlayerManager> {

    #region Fields & Properties

    private readonly Dictionary<ulong, Player> _playerCache = new();
    private readonly Dictionary<ulong, Sprite> _avatarCache = new();

    #endregion

    #region Methods

    #region Public

    public async Task<List<Player>> Search(string? query, int page = 0, int amountPerPage = 20) {

        if (query == null || query.IsEmpty() || query.Length < 2)
            return Enumerable.Empty<Player>().ToList();

        var players = await ApiClient.Instance.Get<List<Player>>($"/players?name={query}&page={page}");
        
        return players ?? Enumerable.Empty<Player>().ToList();

    }

    public PlayerRelationship GetRelationshipWith(Player player) {
        
        if (player.Id == AuthManager.Instance.CurrentPlayer.Id)
            return PlayerRelationship.Self;

        if (FriendManager.Instance.IsFriend(player))
            return PlayerRelationship.Friend;

        if (FriendManager.Instance.HasReceivedRequestFrom(player))
            return PlayerRelationship.ReceivedRequest;

        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (FriendManager.Instance.HasSentRequestTo(player))
            return PlayerRelationship.SentRequest;

        return PlayerRelationship.None;
        
    }

    public IEnumerator DownloadAvatarAndApply(Player player, Image image) {

        if (_avatarCache.TryGetValue(player.Id, out var avatar)) {
            image.sprite = avatar;
            yield break;
        }

        yield return DownloadAvatar(player);

        image.sprite = _avatarCache[player.Id];
        
    }
    
    #endregion

    #region Private

    private IEnumerator DownloadAvatar(Player player) {
        
        var handler = new DownloadHandlerTexture();
        
        var request = new UnityWebRequest(player.AvatarUrl, "GET");
        request.downloadHandler = handler;

        yield return request.SendWebRequest();
        
        if (request.isNetworkError || request.isHttpError)
            Console.WriteLine("Failed to fetch avatar: " + request.error);
        
        _avatarCache.Add(player.Id, Sprite.Create(handler.texture, new Rect(0, 0, handler.texture.width, handler.texture.height), new Vector2(.5f, .5f)));
        
    }
    
    #endregion
    
    #endregion

}