using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barricade : MonoBehaviour
{
    private void Start()
    {
        Destroy(gameObject, 30f);
    }
}
