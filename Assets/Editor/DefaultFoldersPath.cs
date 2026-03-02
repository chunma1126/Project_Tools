using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(DefaultFoldersPath), menuName = "SO/Setting/DefaultFoldersPath")]
public class DefaultFoldersPath : ScriptableObject
{
    public List<String> GetPath() => defaultFoldersPath;

    [SerializeField]
    private List<String> defaultFoldersPath = new List<string>()
    {
        "Script",
        "Prefab",
        "Sprite",
        "Model",
        "Animation",
        "Audio",
        "Material",
        "SO",
        "Scene"
    };

}
