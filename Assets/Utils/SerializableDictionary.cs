using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableKeyValuePair<TKey, TValue>
{
    public TKey key;
    public TValue value;
}

[Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField] private List<SerializableKeyValuePair<TKey, TValue>> _keyValueList =
        new List<SerializableKeyValuePair<TKey, TValue>>();
    
    public void OnBeforeSerialize()
    {
        if (Count < _keyValueList.Count) return;
        
        _keyValueList.Clear();

        foreach (var kvp in this)
        {
            _keyValueList.Add(new SerializableKeyValuePair<TKey, TValue>()
            {
                key = kvp.Key,
                value = kvp.Value
            });
        }
    }

    public void OnAfterDeserialize()
    {
        this.Clear();
        foreach (var kvp in _keyValueList)
        {
            if (this.ContainsKey(kvp.key))
            {
                return;
            }
            
            this.TryAdd(kvp.key, kvp.value);
        }
    }
}