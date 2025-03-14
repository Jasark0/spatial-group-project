using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

public class GameObjectHierarchyComparer : EditorWindow
{
    private GameObject gameObjectA;
    private GameObject gameObjectB;
    private List<string> differences = new List<string>();
    private Vector2 scrollPosition;

    [MenuItem("Tools/GameObject Hierarchy Comparer")]
    public static void ShowWindow()
    {
        GetWindow<GameObjectHierarchyComparer>("GameObject Hierarchy Comparer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Assign the two GameObjects to compare:", EditorStyles.boldLabel);
        gameObjectA = (GameObject)EditorGUILayout.ObjectField("GameObject A", gameObjectA, typeof(GameObject), true);
        gameObjectB = (GameObject)EditorGUILayout.ObjectField("GameObject B", gameObjectB, typeof(GameObject), true);

        if (GUILayout.Button("Compare"))
        {
            if (gameObjectA == null || gameObjectB == null)
            {
                differences.Clear();
                differences.Add("Please assign both GameObjects before comparing.");
            }
            else
            {
                differences.Clear();
                CompareGameObjectHierarchies(gameObjectA.transform, gameObjectB.transform, "");
            }
        }

        GUILayout.Space(10);
        GUILayout.Label("Differences Found:", EditorStyles.boldLabel);

        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(300));
        foreach (string difference in differences)
        {
            EditorGUILayout.HelpBox(difference, MessageType.Warning);
        }
        GUILayout.EndScrollView();
    }

    private void CompareGameObjectHierarchies(Transform transformA, Transform transformB, string path)
    {
        string currentPath = string.IsNullOrEmpty(path) ? transformA.name : $"{path}/{transformA.name}";

        if (transformA.name != transformB.name)
        {
            differences.Add($"Name mismatch at {currentPath}: '{transformA.name}' vs '{transformB.name}'");
        }

        CompareComponents(transformA.gameObject, transformB.gameObject, currentPath);

        int childCountA = transformA.childCount;
        int childCountB = transformB.childCount;

        if (childCountA != childCountB)
        {
            differences.Add($"Child count mismatch at {currentPath}: {childCountA} vs {childCountB}");
        }

        int childCount = Mathf.Min(childCountA, childCountB);

        for (int i = 0; i < childCount; i++)
        {
            CompareGameObjectHierarchies(transformA.GetChild(i), transformB.GetChild(i), currentPath);
        }

        if (childCountA > childCountB)
        {
            for (int i = childCountB; i < childCountA; i++)
            {
                differences.Add($"Extra child in GameObject A at {currentPath}: {transformA.GetChild(i).name}");
            }
        }
        else if (childCountB > childCountA)
        {
            for (int i = childCountA; i < childCountB; i++)
            {
                differences.Add($"Extra child in GameObject B at {currentPath}: {transformB.GetChild(i).name}");
            }
        }
    }

    private void CompareComponents(GameObject objA, GameObject objB, string path)
    {
        Component[] componentsA = objA.GetComponents<Component>();
        Component[] componentsB = objB.GetComponents<Component>();

        foreach (Component compA in componentsA)
        {
            if (compA == null) continue;
            System.Type typeA = compA.GetType();
            Component compB = objB.GetComponent(typeA);
            if (compB == null)
            {
                differences.Add($"Component {typeA} is present in GameObject A but missing in GameObject B at {path}");
            }
            else
            {
                CompareComponentFields(compA, compB, path);
            }
        }

        foreach (Component compB in componentsB)
        {
            if (compB == null) continue;
            System.Type typeB = compB.GetType();
            Component compA = objA.GetComponent(typeB);
            if (compA == null)
            {
                differences.Add($"Component {typeB} is present in GameObject B but missing in GameObject A at {path}");
            }
        }
    }

    private void CompareComponentFields(Component compA, Component compB, string path)
    {
        FieldInfo[] fields = compA.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (FieldInfo field in fields)
        {
            object valueA = field.GetValue(compA);
            object valueB = field.GetValue(compB);
            if (!Equals(valueA, valueB))
            {
                differences.Add($"Difference in {compA.GetType().Name}.{field.Name} at {path}: '{valueA}' vs '{valueB}'");
            }
        }
    }
}
