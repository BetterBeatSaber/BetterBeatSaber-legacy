using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Threading;

using BetterBeatSaber.Core.Utilities;

using UnityEngine;

namespace BetterBeatSaber.Core.Threading; 

public sealed class ThreadDispatcher : LockedConstructableSingleton<ThreadDispatcher> {

    private readonly Thread _thread;
    
    private static readonly ConcurrentQueue<IEnumerator> Queue = new();
    private static readonly ConcurrentQueue<Action> OffMainQueue = new();

    public ThreadDispatcher() {
        _thread = new Thread(RunOffMain) {
            Name = "OffMainThread"
        };
    }

    public static void Enqueue(Action task) {
        Queue.Enqueue(ExecuteTask(task));
    }
    
    public static void Enqueue(IEnumerator routine) {
        Queue.Enqueue(ExecuteTask(routine));
    }
    
    public static void EnqueueOffMain(Action task) {
        OffMainQueue.Enqueue(task);
    }
    
    private static IEnumerator ExecuteTask(Action task) {
        yield return new WaitForEndOfFrame();
        task();
    }
    
    private static IEnumerator ExecuteTask(IEnumerator routine) {
        yield return new WaitForEndOfFrame();
        yield return SharedCoroutineStarter.instance.StartCoroutine(routine);
    }
    
    private IEnumerator Run() {
        while (true) {
            while (Queue.Count > 0) {
                Queue.TryDequeue(out var routine);
                if (routine == null)
                    yield return null;
                yield return routine;
                //yield return SharedCoroutineStarter.instance.StartCoroutine(routine);
            }
            yield return null;
        }
        // ReSharper disable once IteratorNeverReturns
    }
    
    private void RunOffMain() {
        while (true) {
            while (OffMainQueue.Count > 0) {
                OffMainQueue.TryDequeue(out var task);
                task?.Invoke();
            }
        }
        // ReSharper disable once FunctionNeverReturns
    }

    public void Begin() {
        _thread.Start();
        SharedCoroutineStarter.instance.StartCoroutine(Run());
    }
    
    public void End() {
        _thread.Abort();
        SharedCoroutineStarter.instance.StopCoroutine(Run());
    }

}