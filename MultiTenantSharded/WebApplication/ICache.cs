using System;

/// <summary>
/// Representerar en cache, det kan vara något/några annadra färdiga interface som ska användas i praktiken
/// så vi kan inte skicka med tenant i set & get.
/// </summary>
public interface ICache
{
    void Set(string key, object value, DateTimeOffset absoluteExpiration);
    object Get(string key);
    void Invalidate();
}
