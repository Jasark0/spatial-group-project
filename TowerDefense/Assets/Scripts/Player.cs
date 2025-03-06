using UnityEngine;

public class Player : MonoBehaviour
{
    void Update()
    {
        Ray ray = new(Camera.main.transform.position, Camera.main.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, 100))
            if (hit.collider.TryGetComponent<OpenSpot>(out var interactable))
                Debug.Log(interactable.transform.position);
    }
}