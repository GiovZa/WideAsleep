using System.Collections.Generic;
using System;

public class EventCenter
{
    private static Dictionary<EventDefine, Delegate> m_EventTable = new Dictionary<EventDefine, Delegate>();

    private static void OnListenerAdding(EventDefine eventType, Delegate callback)
    {
        if (!m_EventTable.ContainsKey(eventType))
        {
            m_EventTable.Add(eventType, null);
        }
        Delegate d = m_EventTable[eventType];
        if (d != null && d.GetType() != callback.GetType())
        {
            throw new Exception($"Attempting to add listeners for {eventType} with inconsistent types.");
        }
    }

    private static bool OnListenerRemoving(EventDefine eventType, Delegate callback)
    {
        if (m_EventTable.ContainsKey(eventType))
        {
            Delegate d = m_EventTable[eventType];
            if (d == null)
            {
                // Event exists but no listeners - already cleaned up, safe to ignore
                return false;
            }
            else if (d.GetType() != callback.GetType())
            {
                throw new Exception($"Attempting to remove listeners for {eventType} with inconsistent types.");
            }
            return true; // Proceed with removal
        }
        else
        {
            // Event type doesn't exist - likely already cleaned up or never registered
            // This is safe to ignore during cleanup (e.g., OnDisable when game ends)
            return false;
        }
    }

    private static void OnListenerRemoved(EventDefine eventType)
    {
        if (m_EventTable.ContainsKey(eventType))
        {
            m_EventTable.Remove(eventType);
        }
    }

    #region Add Listener
    //no parameter
    public static void AddListener(EventDefine eventType, Callback callback)
    {
        OnListenerAdding(eventType, callback);
        m_EventTable[eventType] = (Callback)m_EventTable[eventType] + callback;
    }

    //one parameter
    public static void AddListener<T>(EventDefine eventType, Callback<T> callback)
    {
        OnListenerAdding(eventType, callback);
        m_EventTable[eventType] = (Callback<T>)m_EventTable[eventType] + callback;
    }

    //two parameters
    public static void AddListener<T, U>(EventDefine eventType, Callback<T, U> callback)
    {
        OnListenerAdding(eventType, callback);
        m_EventTable[eventType] = (Callback<T, U>)m_EventTable[eventType] + callback;
    }

    //three parameters
    public static void AddListener<T, U, V>(EventDefine eventType, Callback<T, U, V> callback)
    {
        OnListenerAdding(eventType, callback);
        m_EventTable[eventType] = (Callback<T, U, V>)m_EventTable[eventType] + callback;
    }

    //four parameters
    public static void AddListener<T, U, V, W>(EventDefine eventType, Callback<T, U, V, W> callback)
    {
        OnListenerAdding(eventType, callback);
        m_EventTable[eventType] = (Callback<T, U, V, W>)m_EventTable[eventType] + callback;
    }
    #endregion

    #region Remove Listener
    //no parameter
    public static void RemoveListener(EventDefine eventType, Callback callback)
    {
        if (!OnListenerRemoving(eventType, callback)) return;
        m_EventTable[eventType] = (Callback)m_EventTable[eventType] - callback;
        OnListenerRemoved(eventType);
    }

    //one parameter
    public static void RemoveListener<T>(EventDefine eventType, Callback<T> callback)
    {
        if (!OnListenerRemoving(eventType, callback)) return;
        m_EventTable[eventType] = (Callback<T>)m_EventTable[eventType] - callback;
        OnListenerRemoved(eventType);
    }

    //two parameters
    public static void RemoveListener<T, U>(EventDefine eventType, Callback<T, U> callback)
    {
        if (!OnListenerRemoving(eventType, callback)) return;
        m_EventTable[eventType] = (Callback<T, U>)m_EventTable[eventType] - callback;
        OnListenerRemoved(eventType);
    }

    //three parameters
    public static void RemoveListener<T, U, V>(EventDefine eventType, Callback<T, U, V> callback)
    {
        if (!OnListenerRemoving(eventType, callback)) return;
        m_EventTable[eventType] = (Callback<T, U, V>)m_EventTable[eventType] - callback;
        OnListenerRemoved(eventType);
    }

    //four parameters
    public static void RemoveListener<T, U, V, W>(EventDefine eventType, Callback<T, U, V, W> callback)
    {
        if (!OnListenerRemoving(eventType, callback)) return;
        m_EventTable[eventType] = (Callback<T, U, V, W>)m_EventTable[eventType] - callback;
        OnListenerRemoved(eventType);
    }
    #endregion

    #region Broadcast
    //no parameter
    public static void Broadcast(EventDefine eventType)
    {
        Delegate d;
        if (m_EventTable.TryGetValue(eventType, out d))
        {
            Callback callback = d as Callback;
            if (callback != null)
            {
                callback();
            }
            else
            {
                throw new Exception($"Attempting to broadcast event {eventType} with inconsistent types.");
            }
        }
    }

    //one parameter
    public static void Broadcast<T>(EventDefine eventType, T arg)
    {
        Delegate d;
        if (m_EventTable.TryGetValue(eventType, out d))
        {
            Callback<T> callback = d as Callback<T>;
            if (callback != null)
            {
                callback(arg);
            }
            else
            {
                throw new Exception($"Attempting to broadcast event {eventType} with inconsistent types.");
            }
        }
    }

    //two parameters
    public static void Broadcast<T, U>(EventDefine eventType, T arg, U arg2)
    {
        Delegate d;
        if (m_EventTable.TryGetValue(eventType, out d))
        {
            Callback<T, U> callback = d as Callback<T, U>;
            if (callback != null)
            {
                callback(arg, arg2);
            }
            else
            {
                throw new Exception($"Attempting to broadcast event {eventType} with inconsistent types.");
            }
        }
    }

    //three parameters
    public static void Broadcast<T, U, V>(EventDefine eventType, T arg, U arg2, V arg3)
    {
        Delegate d;
        if (m_EventTable.TryGetValue(eventType, out d))
        {
            Callback<T, U, V> callback = d as Callback<T, U, V>;
            if (callback != null)
            {
                callback(arg, arg2, arg3);
            }
            else
            {
                throw new Exception($"Attempting to broadcast event {eventType} with inconsistent types.");
            }
        }
    }

    //four parameters
    public static void Broadcast<T, U, V, W>(EventDefine eventType, T arg, U arg2, V arg3, W arg4)
    {
        Delegate d;
        if (m_EventTable.TryGetValue(eventType, out d))
        {
            Callback<T, U, V, W> callback = d as Callback<T, U, V, W>;
            if (callback != null)
            {
                callback(arg, arg2, arg3, arg4);
            }
            else
            {
                throw new Exception($"Attempting to broadcast event {eventType} with inconsistent types.");
            }
        }
    }
    #endregion
}
