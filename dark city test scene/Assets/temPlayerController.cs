using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class temPlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // Movement speed of the character
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Freeze the rotation of the Rigidbody to prevent unwanted tilting.
        rb.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        // Move the character on the Z (forward) and X (sideways) axes
        float horizontalInput = Input.GetAxis("Vertical");
        float verticalInput = Input.GetAxis("Horizontal");
        Vector3 movement = new Vector3(-horizontalInput * moveSpeed, 0f, verticalInput * moveSpeed);
        rb.velocity = movement;
    }
}
