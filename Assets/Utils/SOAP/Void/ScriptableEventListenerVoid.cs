using UnityEngine;
using UnityEngine.Events;

namespace Utils.SOAP
{
    public class ScriptableEventListenerVoid : MonoBehaviour
    {
        [SerializeField] private ScriptableEventVoid scriptableEvent;
        [SerializeField] private UnityEvent response;

        public UnityEventBase Response => response;

        private void OnEnable() => scriptableEvent?.Register(this);
        private void OnDisable() => scriptableEvent?.Unregister(this);

        public void OnEventRaised() => response?.Invoke();
    }
}