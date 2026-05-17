#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;

public class PixelSnapperAutomator : Editor
{
    [MenuItem("Tools/Pixel Art Tools/Setup Pixel Snappers Everywhere")]
    public static void ExecuteSetup()
    {
        int sceneCount = ProcessActiveScene();
        int prefabCount = ProcessProjectPrefabs();

        // 3. Show a nice summary dialog to the developer
        EditorUtility.DisplayDialog(
            "Automation Complete", 
            $"Successfully added UIPixelSnapper to:\n\n" +
            $"• {sceneCount} Text objects in the current Scene.\n" +
            $"• {prefabCount} Prefab assets in the Project folder.", 
            "Great!"
        );
    }

    private static int ProcessActiveScene()
    {
        int count = 0;
        
        // Find all TextMeshProUGUI components in the scene (including inactive ones)
        TextMeshProUGUI[] textComponents = Object.FindObjectsByType<TextMeshProUGUI>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (var tmp in textComponents)
        {
            // Check if the component doesn't already have the snapper script
            if (tmp.GetComponent<UIPixelSnapper>() == null)
            {
                // Undo.AddComponent allows you to Ctrl+Z this operation if needed
                Undo.AddComponent<UIPixelSnapper>(tmp.gameObject);
                count++;
            }
        }

        if (count > 0)
        {
            // Force Unity to acknowledge the scene has changed so it prompts to save
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        return count;
    }

    private static int ProcessProjectPrefabs()
    {
        int count = 0;

        // Find all Prefab GUIDs inside the Assets folder
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" });

        foreach (string guid in prefabGuids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            
            // Modern, safe way to load and edit prefabs under the hood without breaking instances
            GameObject prefabRoot = PrefabUtility.LoadPrefabContents(assetPath);
            
            // Search inside the root of this prefab for text components
            TextMeshProUGUI[] textComponents = prefabRoot.GetComponentsInChildren<TextMeshProUGUI>(true);
            bool prefabModified = false;

            foreach (var tmp in textComponents)
            {
                if (tmp.GetComponent<UIPixelSnapper>() == null)
                {
                    tmp.gameObject.AddComponent<UIPixelSnapper>();
                    prefabModified = true;
                    count++;
                }
            }

            // Only save the prefab back to disk if we actually added a component to it
            if (prefabModified)
            {
                PrefabUtility.SaveAsPrefabAsset(prefabRoot, assetPath);
            }

            // Always unload prefab contents to free up memory
            PrefabUtility.UnloadPrefabContents(prefabRoot);
        }

        // Refresh the database so Unity updates its file meta data
        AssetDatabase.Refresh();
        
        return count;
    }
}
#endif