using BetterBeatSaber.Server.Interfaces;

namespace BetterBeatSaber.Server.Extensions; 

public static class TaskExtensions {

    public static async Task<List<T>> ToSharedModelList<T, E>(this Task<List<E>> task) where E : ISharedConvertable<T> {
        return (await task).ToSharedModelList<T, E>();
    }
    
    public static async Task<List<T>> ToSharedModelList<T, E>(this Task<IEnumerable<E>> task) where E : ISharedConvertable<T> {
        return (await task).ToSharedModelList<T, E>();
    }

}