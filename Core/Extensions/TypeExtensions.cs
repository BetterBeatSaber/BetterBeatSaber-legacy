using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BetterBeatSaber.Core.Extensions; 

public static class TypeExtensions {

    #region Reflection
    
    public const BindingFlags DefaultBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
    
    public static T? Construct<T>(this Type type, IDictionary<string, object>? injections = null) {

        var constructors = type.GetConstructors(DefaultBindingFlags);
        if (constructors.Length == 0)
            return default;

        var constructor = constructors.FirstOrDefault();
        if (constructor == null)
            return default;

        var instance = (T?) constructor.Invoke(new object[] { });
        if (instance == null)
            return default;

        if (injections == null)
            return instance;
        
        #region Properties

        foreach (var property in type.GetProperties(DefaultBindingFlags)) {

            if (!property.CanWrite || !injections.TryGetValue(property.Name, out var value))
                continue;
            
            property.SetValue(instance, value);

        }

        #endregion

        #region Fields

        foreach (var field in type.GetFields(DefaultBindingFlags)) {
            
            if (!injections.TryGetValue(field.Name, out var value))
                continue;
            
            field.SetValue(instance, value);

        }

        #endregion
        
        return instance;

    }

    public static T? GetFieldValue<T>(this Type type, string fieldName, object? instance = null, BindingFlags bindingFlags = DefaultBindingFlags) {
        var field = type.GetField(fieldName, bindingFlags);
        return field != null ? (T?) field.GetValue(instance) : default;
    }
    
    public static T? GetPropertyValue<T>(this Type type, string propertyName, object? instance = null, BindingFlags bindingFlags = DefaultBindingFlags) {
        var property = type.GetProperty(propertyName, bindingFlags);
        return property != null ? (T?) property.GetValue(instance) : default;
    }

    public static T? GetInstance<T>(this Type type) =>
        GetPropertyValue<T>(type, "Instance", bindingFlags: BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
    
    #endregion
    
    #region BSML / UI
    
    internal const string Fallback = "<text text=\"Resource not found\" align=\"Center\"/>";
    
    // ReSharper disable once CollectionNeverUpdated.Local
    private static readonly Dictionary<Type, string> Cache = new();

    public static string GetViewDefinitionResourceName(this Type type) {
        return string.Join(".", type.Namespace, type.Name, "bsml");
    }
    
    public static string ReadViewDefinition(this Type type) {
        if (Cache.TryGetValue(type, out var definition) && definition != null)
            return definition;
        definition = ReadViewDefinitionFromResource(type);
        #if !DEBUG
        Cache[type] = definition;
        #endif
        return definition;
    }
    
    public static string ReadViewDefinitionFromResource(Type type) {
        
        using var stream = type.Assembly.GetManifestResourceStream(type.GetViewDefinitionResourceName());
        if (stream == null)
            return Fallback;
        
        using var streamReader = new StreamReader(stream);
        var resource = streamReader.ReadToEnd();
        
        return resource != string.Empty ? resource : Fallback;

    }
    
    #endregion

}