using System;
using System.Collections.Generic;
using System.Linq;

using BetterBeatSaber.Core.Extensions;
using BetterBeatSaber.Core.Interfaces;

namespace BetterBeatSaber.Core.Manager.Service;

public sealed class ServiceManager : Manager<ServiceManager> {

    private readonly List<Type> _serviceTypes = new();
    private readonly List<Service> _serviceInstances = new();

    private bool _enabled;

    public override void Init() {
        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var serviceType in _serviceTypes)
            BindService(serviceType);
    }

    public override void Exit() {
        
        foreach (var serviceType in _serviceTypes)
            UnregisterService(serviceType);

        _enabled = false;
        
        _serviceTypes.Clear();
        _serviceInstances.Clear();

    }
    
    public override void Enable() {

        foreach (var service in _serviceInstances)
            service.Init();

        foreach (var service in _serviceInstances)
            service.Enable();
        
        _enabled = true;

    }

    public override void Disable() {

        foreach (var service in _serviceInstances)
            service.Disable();
        
        foreach (var service in _serviceInstances)
            service.Exit();

    }
    
    public void RegisterService<T>() where T : Manager.Service.Service {
        RegisterService(typeof(T));
    }

    public void RegisterService(Type type) {
        _serviceTypes.Add(type);
        BindService(type);
    }

    public void UnregisterService<T>() where T : Manager.Service.Service {
        UnregisterService(typeof(T));
    }

    public void UnregisterService(Type type) {
        _serviceInstances.RemoveAll(service => {
            
            if (service.GetType() != type)
                return false;

            service.Exit();

            return true;
        });
        _serviceTypes.Remove(type);
    }

    public T? AcquireService<T>() where T : Manager.Service.Service {
        return (T?) _serviceInstances.FirstOrDefault(service => service.GetType() == typeof(T));
    }

    public object? AcquireService(Type type) {
        return _serviceInstances.FirstOrDefault(service => service.GetType() == type);
    }

    private void BindService(Type type) {

        var service = type.Construct<Manager.Service.Service>();
        if (service == null)
            return;

        if (_enabled) {
            
            service.Init();
            
            if(service is IEnableable machinable)
                machinable.Enable();
            
        }
        
        _serviceInstances.Add(service);

    }

}