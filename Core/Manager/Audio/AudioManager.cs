using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

using BetterBeatSaber.Core.Utilities;

using UnityEngine;

namespace BetterBeatSaber.Core.Manager.Audio; 

public sealed class AudioManager : UnitySingleton<AudioManager> {

    private AudioSource? _audioSource;
    private Coroutine? _coroutine;

    private readonly ConcurrentQueue<AudioClip> _clipQueue = new();
    private readonly ConcurrentQueue<Audio> _audioQueue = new();

    internal static readonly Dictionary<Audio, AudioClip> AudioClips = new();

    public float Volume {
        get => _audioSource != null ? _audioSource.volume : -1f;
        set {
            if (_audioSource != null)
                _audioSource.volume = value;
        }
    }

    public bool IsReady => _audioSource != null;

    #region Unity Event Functions

    private void Start() {
        
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.volume = 1f;
     
        _coroutine = StartCoroutine(Run());

    }

    private void OnDestroy() {
        StopCoroutine(_coroutine);
    }

    #endregion

    #region Methods

    public void PlayAudioClip(AudioClip audioClip) => _clipQueue.Enqueue(audioClip);
    
    public void PlayAudio(Audio audio) => _audioQueue.Enqueue(audio);

    #endregion

    #region Private

    private IEnumerator Run() {
        
        yield return new WaitUntil(() => IsReady);
        
        while (true) {
            
            if(_audioSource!.isPlaying)
                yield return new WaitUntil(() => !_audioSource!.isPlaying);
            
            if (!_clipQueue.IsEmpty) {
                
                if(!_clipQueue.TryDequeue(out var audioClip))
                    yield return new WaitForSeconds(.025f);
                
                _audioSource!.PlayOneShot(audioClip);
                
            } else if (!_audioQueue.IsEmpty) {
                
                if(!_audioQueue.TryDequeue(out var audio))
                    yield return new WaitForSeconds(.025f);

                if (!AudioClips.TryGetValue(audio, out var audioClip))
                    yield return new WaitUntil(() => AudioClips.ContainsKey(audio));
                
                _audioSource!.PlayOneShot(audioClip);

            } else {
                yield return new WaitForSeconds(.01f);
            }
            
        }
        
        // ReSharper disable once IteratorNeverReturns
    }

    #endregion

}