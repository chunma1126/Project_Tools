using System.Collections.Generic;
using UnityEngine;

namespace Utils.SOAP
{
    public abstract class ScriptableEvent<T> : ScriptableObject
    {
        private readonly List<ScriptableEventListener<T>> listeners = new();

        public IReadOnlyList<ScriptableEventListener<T>> Listeners => listeners;

        public void Raise(T value)
        {
            for (int i = listeners.Count - 1; i >= 0; i--)
                listeners[i].OnEventRaised(value);
        }

        public void Register(ScriptableEventListener<T> listener)
        {
            if (!listeners.Contains(listener))
                listeners.Add(listener);
        }

        public void Unregister(ScriptableEventListener<T> listener)
        {
            listeners.Remove(listener);
        }
    }
}
