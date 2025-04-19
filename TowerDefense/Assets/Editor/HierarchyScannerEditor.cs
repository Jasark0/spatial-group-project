using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

public static class HierarchyScannerEditor
{
    [MenuItem("Tools/Scan Hierarchy (Scripts Only)")]
    public static void ScanHierarchy()
    {
        StringBuilder report = new StringBuilder();
        report.AppendLine("=== UNITY HIERARCHY SCRIPT SCAN ===");
        report.AppendLine("(Only shows GameObjects with scripts attached)");
        report.AppendLine("-------------------------------------------");

        // Scan all root GameObjects in the scene
        GameObject[] rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject root in rootObjects)
        {
            ScanGameObject(root, 0, report);
        }

        // Save to a text file
        string filePath = Path.Combine(Application.dataPath, "HierarchyScriptReport.txt");
        File.WriteAllText(filePath, report.ToString());
        EditorUtility.RevealInFinder(filePath); // Open file location
        Debug.Log($"<color=green>Hierarchy scan saved to:</color> {filePath}");
    }

    private static void ScanGameObject(GameObject obj, int indentLevel, StringBuilder report)
    {
        // Check if the GameObject has any scripts
        MonoBehaviour[] scripts = obj.GetComponents<MonoBehaviour>();
        if (scripts.Length > 0)
        {
            // Add indentation for hierarchy visualization
            string indent = new string(' ', indentLevel * 2);
            report.Append($"{indent}└─ {obj.name}");/*  */

            // List all custom scripts (ignoring built-in Unity components)
            foreach (MonoBehaviour script in scripts)
            {
                if (script != null) // Skip missing scripts
                {
                    report.Append($" <color=#6e9ef7>[{script.GetType().Name}]</color>");
                }
            }
            report.AppendLine();
        }

        // Recursively scan children
        foreach (Transform child in obj.transform)
        {
            ScanGameObject(child.gameObject, indentLevel + 1, report);
        }
    }
}