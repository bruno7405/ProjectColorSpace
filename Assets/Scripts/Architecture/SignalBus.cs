using UnityEngine;
using System.Collections.Generic;
using System;

// use signal bus when no information needs to pass other than "hey this happened!"
// designed to work with enums mainly
public static class SignalBus 
{
    private static readonly Dictionary<(Type, int), Action> _signals = new();

    public static void Subscribe<TEnum>(TEnum signal, Action callback) where TEnum : Enum
    {
        var key = (typeof(TEnum), Convert.ToInt32(signal));
        _signals.TryGetValue(key, out var value);
        _signals[key] = value == null ? callback : value + callback;
    }

    public static void Unsubscribe<TEnum>(TEnum signal, Action callback) where TEnum : Enum
    {
        var key = (typeof(TEnum), Convert.ToInt32(signal));
        if (!_signals.TryGetValue(key, out var value)) return;
        var reamining = value - callback;
        if (reamining == null) _signals.Remove(key);
        else _signals[key] = reamining;
    }

    public static void Invoke<TEnum>(TEnum signal) where TEnum : Enum
    {
        var key = (typeof(TEnum), Convert.ToInt32(signal));
        if (_signals.TryGetValue(key, out var value))
            value?.Invoke();
    }
}
