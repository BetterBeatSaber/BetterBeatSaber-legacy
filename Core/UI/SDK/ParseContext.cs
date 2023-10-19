using System;
using System.Collections.Generic;
using System.Linq;

namespace BetterBeatSaber.Core.UI.SDK; 

public sealed class ParseContext {

    public IList<Component> Components { get; private set; } = new List<Component>();

    public Component? GetComponentById(string id) =>
        Components.FirstOrDefault(component => component.Id == id);

    public IEnumerable<T> GetComponentsByType<T>() where T : Component =>
        Components.Where(component => component.GetType() == typeof(T)).Cast<T>();
    
    public IEnumerable<Component> GetComponentsByType(Type type) =>
        Components.Where(component => component.GetType() == type);

}