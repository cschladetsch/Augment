#if UNITY_EDITOR
using System.IO;

using UnityEditor;
using UnityEngine;

internal static class MenuItems
{
    [MenuItem("Assets/Create/Readme.md", false, 81)]
    private static void CreateReadme()
    {
        var obj = Selection.activeObject;
        var dir = obj == null ? "Assets" : AssetDatabase.GetAssetPath(obj.GetInstanceID());
        int number = 0;
        string baseName = "Readme";
        string fileName = baseName;
        while (File.Exists(Path.Combine(dir, $"{fileName}.md")))
        {
            number++;
            fileName = $"{baseName}{number:00}";
        }
        var path = Path.Combine(dir, $"{fileName}.md");
        File.Create(path).Close();
        AssetDatabase.Refresh();
        System.Diagnostics.Process.Start(path);
        Debug.Log($"Created Readme file at: {path}");
    }
}
#endif
