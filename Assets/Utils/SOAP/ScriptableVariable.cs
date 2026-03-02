using UnityEngine;

namespace Utils.SOAP
{
    public abstract class ScriptableVariable<T> : ScriptableObject
    {
        [SerializeField] private T initialValue;
        [System.NonSerialized] private T runtimeValue;
        [System.NonSerialized] private bool initialized;

        public T Value
        {
            get
            {
                if (!initialized) Initialize();
                return runtimeValue;
            }
            set
            {
                runtimeValue = value;
                initialized = true;
                OnValueChanged?.Invoke(value);
            }
        }

        public event System.Action<T> OnValueChanged;

        private void OnEnable()
        {
            Initialize();
        }

        private void Initialize()
        {
            runtimeValue = initialValue;
            initialized = true;
        }

        public void Reset()
        {
            Value = initialValue;
        }

        public static implicit operator T(ScriptableVariable<T> variable) => variable.Value;

        public override string ToString() => Value?.ToString() ?? "null";
    }
}
