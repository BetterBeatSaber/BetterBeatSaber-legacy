using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

using BetterBeatSaber.Core.Extensions;
using BetterBeatSaber.Core.UI.SDK.Attributes;
using BetterBeatSaber.Core.UI.SDK.Components;
using BetterBeatSaber.Core.Utilities;

using UnityEngine;

using TypeExtensions = BetterBeatSaber.Core.Extensions.TypeExtensions;

namespace BetterBeatSaber.Core.UI.SDK; 

public sealed class Parser : ConstructableSingleton<Parser> {

    public readonly Dictionary<string, Type> RegisteredComponents = new();
    
    #region Components

    public void RegisterComponent<T>() where T : Component =>
        RegisteredComponents.Add(typeof(T).Name, typeof(T));

    public void UnregisterComponent<T>() where T : Component =>
        RegisteredComponents.Remove(typeof(T).Name);

    #endregion

    public Parser() {
        RegisterComponent<Avatar>();
        RegisterComponent<Background>();
        RegisterComponent<Button>();
        RegisterComponent<Horizontal>();
        RegisterComponent<Image>();
        RegisterComponent<Panel>();
        RegisterComponent<Tab>();
        RegisterComponent<TabSelector>();
        RegisterComponent<Text>();
        RegisterComponent<Vertical>();
    }

    public void Parse(string raw, GameObject parent, object? host = null) {
        
        var document = new XmlDocument();
        document.LoadXml(raw);

        if (document.DocumentElement == null)
            return;

        var context = new ParseContext();

        Parse(parent.transform, document, context, host);

        foreach (var component in context.Components)
            component.PostCreation(context);
        
    }

    private void Parse(Transform parent, XmlNode xmlNode, ParseContext context, object? host = null) {
        foreach (XmlNode node in xmlNode.ChildNodes) {

            if(node.Name == "xml")
                continue;
            
            if (!RegisteredComponents.TryGetValue(node.Name, out var componentType))
                throw new Exception("Failed to find matching Component");

            var props = new Dictionary<string, object?>();

            if (node.Attributes != null) {
                foreach (var propertyInfo in componentType.GetProperties(TypeExtensions.DefaultBindingFlags)) {

                    var ignorePropertyAttribute = propertyInfo.GetCustomAttribute<IgnorePropertyAttribute>();
                    if(ignorePropertyAttribute != null)
                        continue;
                    
                    var attribute = node.Attributes[propertyInfo.Name];
                    if(attribute == null)
                        continue;

                    props[propertyInfo.Name] = ChangeType(attribute.Value, propertyInfo.PropertyType);

                }
            }

            var component = componentType.Construct<Component>(props);
            if(component == null)
                throw new Exception("Failed to construct Component");

            var gameObject = component.Create(parent, node);

            // just because it isn't automatically everywhere
            if (component.GameObject == null)
                component.GameObject = gameObject;

            if (host != null) {
                // todo: inject components based on id and type if there is only one
                // and register button/action handlers to component ...
            }
            
            if (component is Component.IParent)
                Parse(gameObject.transform, node, context, host);
            
            context.Components.Add(component);
            
        }
    }
    
    private static object? ChangeType(object? value, Type conversion) {

        if (!conversion.IsGenericType || conversion.GetGenericTypeDefinition() != typeof(Nullable<>))
            return Convert.ChangeType(value, conversion);
        
        return value != null ? Convert.ChangeType(value, Nullable.GetUnderlyingType(conversion)!) : null;

    }

}