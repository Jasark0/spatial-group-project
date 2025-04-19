using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

public class HierarchyScannerWindow : EditorWindow
{
    private GameObject targetGameObject;
    private Vector2 scrollPosition;
    private string hierarchyReport = "";
    private bool showPreview = true;
    
    [MenuItem("Tools/Hierarchy Scanner Window")]
    public static void ShowWindow()
    {
        HierarchyScannerWindow window = GetWindow<HierarchyScannerWindow>();
        window.titleContent = new GUIContent("Hierarchy Scanner");
        window.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("GameObject Hierarchy Scanner", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);
        
        // Object field for dragging the GameObject
        targetGameObject = (GameObject)EditorGUILayout.ObjectField(
            "Target GameObject:", 
            targetGameObject, 
            typeof(GameObject), 
            true
        );
        
        EditorGUILayout.Space(10);
        
        using (new EditorGUI.DisabledScope(targetGameObject == null))
        {
            if (GUILayout.Button("Scan Hierarchy", GUILayout.Height(30)))
            {
                ScanTargetGameObject();
            }
        }
        
        EditorGUILayout.Space(5);
        
        if (!string.IsNullOrEmpty(hierarchyReport))
        {
            EditorGUILayout.BeginHorizontal();
            showPreview = EditorGUILayout.Toggle("Show Preview", showPreview);
            
            if (GUILayout.Button("Save to File"))
            {
                SaveReportToFile();
            }
            
            if (GUILayout.Button("Copy to Clipboard"))
            {
                EditorGUIUtility.systemCopyBuffer = hierarchyReport;
                Debug.Log("Hierarchy report copied to clipboard.");
            }
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(5);
            
            if (showPreview)
            {
                EditorGUILayout.LabelField("Hierarchy Preview:", EditorStyles.boldLabel);
                
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true));
                EditorGUILayout.TextArea(hierarchyReport, GUILayout.ExpandHeight(true));
                EditorGUILayout.EndScrollView();
            }
        }
    }

    private void ScanTargetGameObject()
    {
        if (targetGameObject == null)
            return;
            
        StringBuilder report = new StringBuilder();
        report.AppendLine($"=== HIERARCHY SCAN: {targetGameObject.name} ===");
        report.AppendLine("-------------------------------------------");
        
        ScanGameObject(targetGameObject, 0, report);
        
        hierarchyReport = report.ToString();
    }
    
    private void SaveReportToFile()
    {
        string defaultName = $"{targetGameObject.name}_HierarchyReport.txt";
        string filePath = EditorUtility.SaveFilePanel(
            "Save Hierarchy Report", 
            Application.dataPath, 
            defaultName, 
            "txt"
        );
        
        if (!string.IsNullOrEmpty(filePath))
        {
            File.WriteAllText(filePath, hierarchyReport);
            Debug.Log($"Hierarchy saved to: {filePath}");
            
            // If the path is within the Assets folder, refresh the AssetDatabase
            if (filePath.StartsWith(Application.dataPath))
            {
                AssetDatabase.Refresh();
            }
        }
    }

    private static void ScanGameObject(GameObject obj, int indentLevel, StringBuilder report)
    {
        // Add indentation for hierarchy visualization
        string indent = new string(' ', indentLevel * 2);
        report.Append($"{indent}└─ {obj.name}");
        
        // List all scripts
        MonoBehaviour[] scripts = obj.GetComponents<MonoBehaviour>();
        if (scripts.Length > 0)
        {
            foreach (MonoBehaviour script in scripts)
            {
                if (script != null) // Skip missing scripts
                {
                    report.Append($" [{script.GetType().Name}]");
                }
            }
        }
        
        report.AppendLine();
        
        // Recursively scan children
        foreach (Transform child in obj.transform)
        {
            ScanGameObject(child.gameObject, indentLevel + 1, report);
        }
    }
}