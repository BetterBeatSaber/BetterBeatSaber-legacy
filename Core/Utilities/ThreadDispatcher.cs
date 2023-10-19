using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

using UnityEngine;

namespace BetterBeatSaber.Core.Utilities; 

public sealed class ThreadDispatcher : LockedConstructableSingleton<ThreadDispatcher> {

    private readonly Thread _thread;
    
    private static readonly ConcurrentQueue<IEnumerator> Queue = new();
    private static readonly ConcurrentQueue<Action> OffMainQueue = new();

    public ThreadDispatcher() {
        _thread = new Thread(RunOffMain) {
            Name = "OffMainThread"
        };
    }

    #region Init & Exit

    public void Init() {
        _thread.Start();
        SharedCoroutineStarter.instance.StartCoroutine(Run());
    }
    
    public void Exit() {
        _thread.Abort();
        SharedCoroutineStarter.instance.StopCoroutine(Run());
    }

    #endregion
    
    #region Main

    public static void Enqueue(Action action) =>
        Queue.Enqueue(ExecuteTask(action));
    
    public static void Enqueue(IEnumerator routine) =>
        Queue.Enqueue(ExecuteTask(routine));
    
    public static void Enqueue(Task task) =>
        Queue.Enqueue(ExecuteTask(task));
    
    #endregion
    
    #region Off-Main

    public static void EnqueueOffMain(Action action) =>
        OffMainQueue.Enqueue(action);
    
    public static void EnqueueOffMain(Task task) =>
        OffMainQueue.Enqueue(task.Start);
    
    #endregion

    #region Execute

    private static IEnumerator ExecuteTask(Action task) {
        yield return new WaitForEndOfFrame();
        task();
    }
    
    private static IEnumerator ExecuteTask(IEnumerator routine) {
        yield return new WaitForEndOfFrame();
        yield return SharedCoroutineStarter.instance.StartCoroutine(routine);
    }

    private static IEnumerator ExecuteTask(Task task) {
        yield return new WaitForEndOfFrame();
        task.Start();
        yield return new WaitUntil(() => task.IsCompleted);
    }
    
    #endregion

    #region Run

    private static IEnumerator Run() {
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
    
    private static void RunOffMain() {
        while (true) {
            while (OffMainQueue.Count > 0) {
                OffMainQueue.TryDequeue(out var task);
                task?.Invoke();
            }
        }
        // ReSharper disable once FunctionNeverReturns
    }

    #endregion
    
}