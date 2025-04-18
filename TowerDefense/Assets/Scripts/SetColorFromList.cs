using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetColorFromList : MonoBehaviour
{
    public List<Color> colors;
    public void SetColor(int i)
    {
        Debug.Log("SetColor: " + i);
        GetComponent<Renderer>().material.color = colors[i];
    }
}
