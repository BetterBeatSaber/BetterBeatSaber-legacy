using BetterBeatSaber.Server.Interfaces;

namespace BetterBeatSaber.Server.Extensions; 

public static class EnumerableExtensions {

    public static List<T> ToSharedModelList<T, E>(this IEnumerable<E> enumerable) where E : ISharedConvertable<T> =>
        enumerable.Select(element => element.ToSharedModel()).ToList();

}