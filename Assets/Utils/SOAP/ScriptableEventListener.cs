using UnityEngine;
using UnityEngine.Events;

namespace Utils.SOAP
{
    public abstract class ScriptableEventListener<T> : MonoBehaviour
    {
        [SerializeField] private ScriptableEvent<T> scriptableEvent;
        [SerializeField] private UnityEvent<T> response;

        public UnityEventBase Response => response;

        private void OnEnable() => scriptableEvent?.Register(this);
        private void OnDisable() => scriptableEvent?.Unregister(this);

        public void OnEventRaised(T value) => response?.Invoke(value);
    }
}
