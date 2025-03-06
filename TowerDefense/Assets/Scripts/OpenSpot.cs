using UnityEngine;
public class OpenSpot : MonoBehaviour
{
    public void Init(Vector3Int coords)
    {
        Debug.Log(coords);
    }
    void Update()
    {

    }
}
/*
for each tile in grid:
    at start make an object from prefab that has pickable script + collider
in pickable script on ray from hand to collider if collide change color
    store object placed on there or null
    if store object not placed, signal ui to pop up

TODO: make pickable script
    update start in grid
*/