using System;
using System.Collections.Generic;
using UnityEngine;

// use event bus when you need to pass more info along with the signal
// typically I use a little struct and pass it along
public static class EventBus 
{
    private static readonly Dictionary<Type, Delegate> _events = new();

    public static void Subscribe<T>(Action<T> callback)
    {
        _events.TryGetValue(typeof(T), out var value);
        _events[typeof(T)] = Delegate.Combine(value, callback);
    }

    public static void Unsubscribe<T>(Action<T> callback)
    {
        if (!_events.TryGetValue(typeof(T), out var value)) return;
        var reamining = Delegate.Remove(value, callback);
        if (reamining == null) _events.Remove(typeof(T));
        else _events[typeof(T)] = reamining;
    }

    public static void Invoke<T>(T eventData)
    {
        if (_events.TryGetValue(typeof(T), out var value))
        {
            ((Action<T>)value).Invoke(eventData);
        }
    }
}
