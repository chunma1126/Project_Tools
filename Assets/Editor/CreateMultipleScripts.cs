using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class CreateMultipleScripts : EditorWindow
{
    [System.Serializable]
    private class ScriptEntry
    {
        public string name = "";
        public string templateName = "";
    }

    private List<ScriptEntry> scriptEntries = new List<ScriptEntry> { new ScriptEntry() };
    private List<string> templateNames = new List<string>();

    private string folderPath = "Assets";
    private Vector2 scrollPosition;

    private string newTemplateFileName = "";

    [MenuItem("Assets/Create/C# Scripts(Multiple)")]
    public static void ShowWindow()
    {
        var window = GetWindow<CreateMultipleScripts>("Create Multiple Scripts");
        window.minSize = new Vector2(450, 300);
        window.folderPath = GetSelectedFolderPath();
        window.RefreshTemplates();
    }

    private void OnEnable()
    {
        RefreshTemplates();
    }

    private static string GetSelectedFolderPath()
    {
        var selected = Selection.GetFiltered<Object>(SelectionMode.Assets);
        if (selected.Length == 0) return "Assets";

        string path = AssetDatabase.GetAssetPath(selected[0]);
        if (!Directory.Exists(path))
            path = Path.GetDirectoryName(path);

        return path;
    }

    private void OnGUI()
    {
        EditorGUILayout.Space(10);

        // Template Add
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("템플릿 추가", GUILayout.Width(80));
        newTemplateFileName = EditorGUILayout.TextField(newTemplateFileName);

        if (GUILayout.Button("Add", GUILayout.Width(80)))
        {
            CreateNewTemplate();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(10);

        // Folder
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("생성 위치", GUILayout.Width(60));
        folderPath = EditorGUILayout.TextField(folderPath);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("스크립트 목록", EditorStyles.boldLabel);

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        int removeIndex = -1;

        for (int i = 0; i < scriptEntries.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            int index = Mathf.Max(0, templateNames.IndexOf(scriptEntries[i].templateName));
            index = EditorGUILayout.Popup(index, templateNames.ToArray(), GUILayout.Width(150));
            scriptEntries[i].templateName = templateNames.Count > 0 ? templateNames[index] : "";

            scriptEntries[i].name = EditorGUILayout.TextField(scriptEntries[i].name);

            if (GUILayout.Button("-", GUILayout.Width(25)))
                removeIndex = i;

            EditorGUILayout.EndHorizontal();
        }

        if (removeIndex >= 0 && scriptEntries.Count > 1)
            scriptEntries.RemoveAt(removeIndex);

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("+", GUILayout.Width(25)))
            scriptEntries.Add(new ScriptEntry());
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space(10);

        if (GUILayout.Button("Create", GUILayout.Height(30)))
            CreateScripts();
    }

    private void RefreshTemplates()
    {
        string folder = "Assets/Editor/ScriptTemplates/";

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        templateNames = Directory.GetFiles(folder, "*.txt")
            .Select(Path.GetFileNameWithoutExtension)
            .ToList();
    }

    private void CreateScripts()
    {
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            AssetDatabase.Refresh();
        }

        int createdCount = 0;

        foreach (var entry in scriptEntries)
        {
            string trimmedName = entry.name.Trim();
            if (string.IsNullOrEmpty(trimmedName))
                continue;

            if (string.IsNullOrEmpty(entry.templateName))
                continue;

            string filePath = Path.Combine(folderPath, trimmedName + ".cs");

            if (File.Exists(filePath))
            {
                Debug.LogWarning("이미 존재: " + filePath);
                continue;
            }

            string content = LoadTemplateFromFile(entry.templateName, trimmedName);
            File.WriteAllText(filePath, content);
            createdCount++;
        }

        AssetDatabase.Refresh();

        if (createdCount > 0)
            Close();
        else
            EditorUtility.DisplayDialog("알림", "생성할 스크립트가 없습니다.", "OK");
    }

    private string LoadTemplateFromFile(string templateName, string className)
    {
        string templatePath = Path.Combine("Assets/Editor/ScriptTemplates/", templateName + ".txt");

        if (!File.Exists(templatePath))
        {
            Debug.LogError("Template not found: " + templatePath);
            return "";
        }

        string templateContent = File.ReadAllText(templatePath);
        return templateContent.Replace("{{className}}", className);
    }

    private void CreateNewTemplate()
    {
        if (string.IsNullOrEmpty(newTemplateFileName))
            return;

        if (!newTemplateFileName.EndsWith(".txt"))
            newTemplateFileName += ".txt";

        string path = Path.Combine("Assets/Editor/ScriptTemplates/", newTemplateFileName);

        Directory.CreateDirectory(Path.GetDirectoryName(path));

        if (!File.Exists(path))
        {
            File.WriteAllText(path,
@"using UnityEngine;

        public class {{className}}
        {
    
        }");
        }

        newTemplateFileName = "";
        AssetDatabase.Refresh();
        RefreshTemplates();
    }
}
