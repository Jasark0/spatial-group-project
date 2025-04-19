using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Movement speed in units per second")]
    public float moveSpeed = 5f;

    private Vector3 moveDirection;
    private CharacterController characterController;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        // Try to get CharacterController first (preferred)
        characterController = GetComponentInChildren<CharacterController>();
        
        // If no CharacterController, try to use Rigidbody
        if (characterController == null)
        {
            rb = GetComponent<Rigidbody>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Get input for horizontal and vertical movement
        float horizontalInput = Input.GetAxisRaw("Horizontal"); // A and D keys
        float verticalInput = Input.GetAxisRaw("Vertical");     // W and S keys
        
        // Create movement direction vector
        moveDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;
        
        // Apply movement if using CharacterController
        if (characterController != null)
        {
            characterController.Move(moveDirection * moveSpeed * Time.deltaTime);
        }
    }
    
    // For physics-based movement with Rigidbody
    void FixedUpdate()
    {
        if (characterController == null && rb != null)
        {
            // Apply movement force to rigidbody
            Vector3 movement = moveDirection * moveSpeed;
            rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);
        }
    }
}
