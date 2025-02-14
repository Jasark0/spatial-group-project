using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class SortChildrenByName
{
    [MenuItem("Tools/Sort Selected Parent's Children By Name")]
    public static void Sort()
    {
        foreach (Transform parent in Selection.transforms)
        {
            List<Transform> children = parent.Cast<Transform>().ToList();
            children.Sort((t1, t2) => string.Compare(t1.name, t2.name));
            for (int i = 0; i < children.Count; i++)
            {
                Undo.SetTransformParent(children[i], parent, "Sort Children");
                children[i].SetSiblingIndex(i);
            }
        }
    }
}