#if UNITY_EDITOR

namespace Augment
{
    using System.Linq;

    using UnityEditor;
    using UnityEngine;

    public static class EditorUtility
    {
        private static T[] LoadAssetsAtPath<T>(string filter, string[] searchInFolders)
            where T : Object
        {
            return AssetDatabase.FindAssets(filter, searchInFolders)
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<T>)
                .ToArray();
        }
    }
}

#endif // UNITY_EDITOR

