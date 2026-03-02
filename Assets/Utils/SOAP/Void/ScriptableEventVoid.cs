using System.Collections.Generic;
using UnityEngine;

namespace Utils.SOAP
{
    [CreateAssetMenu(fileName = "ScriptableEventVoid", menuName = "SO/SOAP/ScriptableEventVoid")]
    public class ScriptableEventVoid : ScriptableObject
    {
        private readonly List<ScriptableEventListenerVoid> listeners = new();

        public IReadOnlyList<ScriptableEventListenerVoid> Listeners => listeners;

        public void Raise()
        {
            for (int i = listeners.Count - 1; i >= 0; i--)
                listeners[i].OnEventRaised();
        }

        public void Register(ScriptableEventListenerVoid listener)
        {
            if (!listeners.Contains(listener))
                listeners.Add(listener);
        }

        public void Unregister(ScriptableEventListenerVoid listener)
        {
            listeners.Remove(listener);
        }
    }
}