using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BillboardText : MonoBehaviour
{
    private Transform cameraTransform;

    [SerializeField] private Transform targetTower; // The main tower to revolve around
    [SerializeField] private float distance = 2.0f; // Distance from the tower
    [SerializeField] private float heightOffset = 0f; // Height offset from the tower's position
    [SerializeField] private float angleOffset = 0f; // Angle offset in degrees (positive = clockwise)
    [SerializeField] private bool centerText = true; // Whether to center the text by offsetting half its width
    [SerializeField] private Color gizmoColor = new Color(0.2f, 0.8f, 0.2f, 0.5f); // Color for the gizmo


    // Look for the main camera on start
    void Start()
    {
        FindCamera();

        // Initialize height offset if not set
        if (heightOffset == 0)
        {
            heightOffset = transform.position.y - targetTower.position.y;
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (cameraTransform == null)
        {
            // If camera was not found in Start, try again
            FindCamera();
            return;
        }

        // Calculate the direction from tower to camera (horizontal only)
        Vector3 towerToCamera = cameraTransform.position - targetTower.position;
        towerToCamera.y = 0; // Zero out vertical component

        if (towerToCamera.magnitude > 0.001f) // Prevent division by zero
        {
            // Normalize and use the direction to position the text at the correct distance
            Vector3 direction = towerToCamera.normalized;

            // Apply angle offset if needed
            if (angleOffset != 0)
            {
                // Rotate the direction vector around the Y axis by the specified angle
                direction = Quaternion.Euler(0, angleOffset, 0) * direction;
            }

            // Position the text at the specified distance from the tower in the modified direction
            Vector3 newPosition = targetTower.position + (direction * distance);

            // Apply height offset
            newPosition.y = targetTower.position.y + heightOffset;

            // Update the text position and make it face the camera, but only horizontally
            // Note we want it to face toward the player, so we use the negative of the direction
            transform.SetPositionAndRotation(newPosition, Quaternion.LookRotation(-direction, Vector3.up));
        }
    }

    private void FindCamera()
    {
        var vrCamera = FindObjectOfType<OVRCameraRig>();
        if (vrCamera != null && vrCamera.centerEyeAnchor != null)
        {
            cameraTransform = vrCamera.centerEyeAnchor;
        }
        else
        {
            cameraTransform = Camera.main.transform;
        }
    }

    // Draw gizmos to visualize the circle in the editor
    private void OnDrawGizmos()
    {
        if (targetTower == null)
        {
            // Try to find target tower for visualization in editor
            if (!Application.isPlaying)
            {
                targetTower = GameObject.FindWithTag("MainTower")?.transform;
                if (targetTower == null) return;
            }
            else
            {
                return;
            }
        }

        // Set gizmo color
        Gizmos.color = gizmoColor;

        // Get height offset for visualization
        float yOffset = heightOffset;
        if (yOffset == 0 && !Application.isPlaying)
        {
            yOffset = transform.position.y - targetTower.position.y;
        }

        // Draw the circle representing the possible positions
        DrawGizmoCircle(targetTower.position, distance, yOffset, 32);

        // In play mode, draw a line from the tower to the text
        if (Application.isPlaying)
        {
            Gizmos.DrawLine(targetTower.position, transform.position);

            // Draw a ray in the facing direction
            Vector3 facingDirection = transform.forward * 0.5f;
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, facingDirection);
        }
        else if (Camera.current != null)
        {
            // In editor mode, show example position based on scene view camera
            Vector3 editorCameraPos = Camera.current.transform.position;
            Vector3 towerToCamera = editorCameraPos - targetTower.position;
            towerToCamera.y = 0;

            if (towerToCamera.magnitude > 0.001f)
            {
                Vector3 direction = towerToCamera.normalized;
                
                // Apply angle offset for the preview
                if (angleOffset != 0)
                {
                    direction = Quaternion.Euler(0, angleOffset, 0) * direction;
                }
                
                Vector3 previewPos = targetTower.position + (direction * distance);
                previewPos.y = targetTower.position.y + yOffset;

                Gizmos.DrawLine(targetTower.position, previewPos);
                Gizmos.DrawSphere(previewPos, 0.1f);
                
                // Draw a small line showing the facing direction
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(previewPos, -direction * 0.3f);
            }
        }
    }

    // Helper method to draw a circle with gizmos
    private void DrawGizmoCircle(Vector3 center, float radius, float yOffset, int segments)
    {
        // Draw the circle segments
        Vector3 prevPoint = center + new Vector3(radius, yOffset, 0);
        for (int i = 1; i <= segments; i++)
        {
            float angle = i * 2 * Mathf.PI / segments;
            Vector3 point = center + new Vector3(
                Mathf.Cos(angle) * radius,
                yOffset,
                Mathf.Sin(angle) * radius
            );
            Gizmos.DrawLine(prevPoint, point);
            prevPoint = point;
        }
    }
}