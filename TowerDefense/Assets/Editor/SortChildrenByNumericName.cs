using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public class SortChildrenByNumericName
{
    [MenuItem("Tools/Sort Selected Parent's Children By Numeric Name")]
    public static void Sort()
    {
        foreach (Transform parent in Selection.transforms)
        {
            List<Transform> children = parent.Cast<Transform>().ToList();
            children.Sort((t1, t2) =>
            {
                int num1 = ExtractNumber(t1.name);
                int num2 = ExtractNumber(t2.name);
                return num1.CompareTo(num2);
            });
            for (int i = 0; i < children.Count; i++)
            {
                Undo.SetTransformParent(children[i], parent, "Sort Children");
                children[i].SetSiblingIndex(i);
            }
        }
    }

    private static int ExtractNumber(string name)
    {
        Match match = Regex.Match(name, @"\d+");
        return match.Success ? int.Parse(match.Value) : int.MaxValue;
    }
}