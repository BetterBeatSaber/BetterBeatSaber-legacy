using System.Reflection;

namespace BetterBeatSaber.Core.Extensions; 

// ReSharper disable InconsistentNaming

public static class ObjectExtensions {

    public static void SetField<O, T>(this O obj, string fieldName, T value, BindingFlags bindingFlags = TypeExtensions.DefaultBindingFlags) {
        typeof(O).GetField(fieldName, bindingFlags)?.SetValue(obj, value);
    }

    public static T? GetField<O, T>(this O obj, string fieldName, BindingFlags bindingFlags = TypeExtensions.DefaultBindingFlags) {
        return (T?) typeof(O).GetField(fieldName, bindingFlags)?.GetValue(obj);
    }
    
    public static void SetProperty<O, T>(this O obj, string propertyName, T value, BindingFlags bindingFlags = TypeExtensions.DefaultBindingFlags) {
        typeof(O).GetProperty(propertyName, bindingFlags)?.SetValue(obj, value);
    }

    public static T? GetProperty<O, T>(this O obj, string propertyName, BindingFlags bindingFlags = TypeExtensions.DefaultBindingFlags) {
        return (T?) typeof(O).GetProperty(propertyName, bindingFlags)?.GetValue(obj);
    }

}